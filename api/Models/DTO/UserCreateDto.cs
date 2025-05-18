using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class UserCreateDto
    {
        public string? ResidentName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? ApartmentInformation { get; set; }
    }
}
