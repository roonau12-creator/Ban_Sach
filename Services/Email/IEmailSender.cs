using BanSach.Models;

namespace BanSach.Services.Email
{
    public interface IEmailSender
    {
        Task SendAsync(EmailOutbox message, CancellationToken cancellationToken);
    }
}
