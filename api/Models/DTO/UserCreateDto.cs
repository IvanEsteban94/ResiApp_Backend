using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class UserCreateDto
    {

        public string? ResidentName { get; set; }

        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "The field Password must be a string or array type with a minimum length of '6'.")]
        public string? Password { get; set; }

        public string? ApartmentInformation { get; set; }

        public string? Role { get; set; }
        public string? SecurityWord { get; set; }
       
        public string? ImageBase64
        {
            get; set;
        }


    }
}
