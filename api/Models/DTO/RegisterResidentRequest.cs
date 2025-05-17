using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class RegisterResidentRequest
    {
        [Required]
        public string ResidentName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string ApartmentInformation { get; set; }
    }
}
