using System.ComponentModel.DataAnnotations;

namespace BanSach.Models;

public class ApplicationUser
{
    public int Id { get; set; }

    [Required, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(256)]
    public string NormalizedEmail { get; set; } = string.Empty;

    [Required, StringLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, StringLength(32)]
    public string Role { get; set; } = UserRoles.Customer;

    public DateTime CreatedAtUtc { get; set; }
}
