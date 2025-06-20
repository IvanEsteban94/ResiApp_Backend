using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string Role { get; set; } = "Resident";

    public string? ResidentName { get; set; }
    public string? ApartmentInformation { get; set; }

    [Required]
    public string? SecurityWord { get; set; } = null!;
}
