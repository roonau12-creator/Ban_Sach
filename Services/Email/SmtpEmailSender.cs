using System.Net;
using System.Net.Mail;
using BanSach.Models;
using Microsoft.Extensions.Options;

namespace BanSach.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(EmailOutbox message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpHost) ||
                string.IsNullOrWhiteSpace(_settings.FromAddress))
            {
                throw new InvalidOperationException("SMTP host and sender address must be configured.");
            }

            var from = new MailAddress(_settings.FromAddress, _settings.FromName);
            var to = new MailAddress(message.Recipient);
            using var mail = new MailMessage(from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = false
            };
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl
            };

            if (!string.IsNullOrWhiteSpace(_settings.Username))
            {
                if (string.IsNullOrWhiteSpace(_settings.Password))
                {
                    throw new InvalidOperationException("SMTP password must be configured when a username is set.");
                }

                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            }
            else if (!string.IsNullOrWhiteSpace(_settings.Password))
            {
                throw new InvalidOperationException("SMTP username must be configured when a password is set.");
            }

            await client.SendMailAsync(mail, cancellationToken);
        }
    }
}
