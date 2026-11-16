using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class EmailOutbox
    {
        [Key]
        public int MaEmail { get; set; }

        [Required]
        [StringLength(120)]
        public string EventKey { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Recipient { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = EmailOutboxStatus.Pending;

        public int AttemptCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime NextAttemptAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessingAt { get; set; }
        public DateTime? SentAt { get; set; }

        [StringLength(2000)]
        public string? LastError { get; set; }
    }

    public static class EmailOutboxStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Sent = "Sent";
        public const string Failed = "Failed";
    }
}
