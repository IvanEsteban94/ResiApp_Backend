using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Admin : IUser
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
