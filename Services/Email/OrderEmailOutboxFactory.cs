using BanSach.Models;

namespace BanSach.Services.Email
{
    public static class OrderEmailOutboxFactory
    {
        public static EmailOutbox Confirmation(DonHang order, string recipient)
        {
            var email = OrderEmailTemplate.Confirmation(order);
            return Create($"order:{order.MaDH}:created", recipient, email.Subject, email.Body);
        }

        public static EmailOutbox StatusChanged(
            DonHang order,
            string previousStatus,
            string newStatus,
            string recipient)
        {
            var email = OrderEmailTemplate.StatusChanged(order, previousStatus, newStatus);
            var eventKey = $"order:{order.MaDH}:status:{order.TrangThaiVersion}";
            return Create(eventKey, recipient, email.Subject, email.Body);
        }

        private static EmailOutbox Create(string eventKey, string recipient, string subject, string body)
        {
            var now = DateTime.UtcNow;
            return new EmailOutbox
            {
                EventKey = eventKey,
                Recipient = recipient,
                Subject = subject,
                Body = body,
                CreatedAt = now,
                NextAttemptAt = now
            };
        }
    }
}
