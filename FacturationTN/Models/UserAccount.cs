using System.ComponentModel.DataAnnotations;

namespace FacturationTN.Models;

public class UserAccount
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NormalizedUserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "User";

    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    [Phone]
    public string? Phone { get; set; }

    [MaxLength(4000000)]
    public string? ProfileImageDataUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
