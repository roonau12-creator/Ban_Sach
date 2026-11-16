using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Services.Email
{
    public class EmailOutboxWorker : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan StaleProcessingInterval = TimeSpan.FromMinutes(5);
        private const int MaxAttempts = 5;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailOutboxWorker> _logger;

        public EmailOutboxWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<EmailOutboxWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var processed = await ProcessOneAsync(stoppingToken);
                    if (!processed)
                    {
                        await Task.Delay(PollInterval, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Failed to process the email outbox.");
                    await Task.Delay(PollInterval, stoppingToken);
                }
            }
        }

        private async Task<bool> ProcessOneAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = DateTime.UtcNow;
            var staleBefore = now - StaleProcessingInterval;
            var messageId = await context.EmailOutbox
                .Where(x =>
                    (x.Status == EmailOutboxStatus.Pending && x.NextAttemptAt <= now) ||
                    (x.Status == EmailOutboxStatus.Processing && x.ProcessingAt < staleBefore))
                .OrderBy(x => x.CreatedAt)
                .Select(x => x.MaEmail)
                .FirstOrDefaultAsync(cancellationToken);

            if (messageId == 0)
            {
                return false;
            }

            var claimed = await context.EmailOutbox
                .Where(x => x.MaEmail == messageId &&
                    ((x.Status == EmailOutboxStatus.Pending && x.NextAttemptAt <= now) ||
                     (x.Status == EmailOutboxStatus.Processing && x.ProcessingAt < staleBefore)))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, EmailOutboxStatus.Processing)
                    .SetProperty(x => x.ProcessingAt, now)
                    .SetProperty(x => x.AttemptCount, x => x.AttemptCount + 1), cancellationToken);

            if (claimed == 0)
            {
                return true;
            }

            var message = await context.EmailOutbox
                .AsNoTracking()
                .SingleAsync(x => x.MaEmail == messageId, cancellationToken);

            try
            {
                var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                await sender.SendAsync(message, cancellationToken);

                await context.EmailOutbox
                    .Where(x => x.MaEmail == messageId && x.Status == EmailOutboxStatus.Processing)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, EmailOutboxStatus.Sent)
                        .SetProperty(x => x.SentAt, DateTime.UtcNow)
                        .SetProperty(x => x.ProcessingAt, (DateTime?)null)
                        .SetProperty(x => x.LastError, (string?)null), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to send outbox email {EmailId}.", messageId);
                var retryAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, message.AttemptCount));
                var error = exception.Message.Length <= 2000
                    ? exception.Message
                    : exception.Message[..2000];
                var finalStatus = message.AttemptCount >= MaxAttempts
                    ? EmailOutboxStatus.Failed
                    : EmailOutboxStatus.Pending;

                await context.EmailOutbox
                    .Where(x => x.MaEmail == messageId && x.Status == EmailOutboxStatus.Processing)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, finalStatus)
                        .SetProperty(x => x.NextAttemptAt, retryAt)
                        .SetProperty(x => x.ProcessingAt, (DateTime?)null)
                        .SetProperty(x => x.LastError, error), cancellationToken);
            }

            return true;
        }
    }
}
