using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Mythral.Server.Domain.Enums;

namespace Mythral.Server.Domain.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Cellphone), IsUnique = true)]
[Index(nameof(Status))]
public class Account
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [MaxLength(32)]
    public string Cellphone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ConcurrencyCheck]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}