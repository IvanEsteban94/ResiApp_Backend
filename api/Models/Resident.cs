using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Resident : IUser
    {
        public int Id { get; set; }
        [Required]
        public string ResidentName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Resident";
        public string ApartmentInformation { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Getter y setter de respaldo (si decides mantener Password)
        public string Password
        {
            get => PasswordHash;
            set => PasswordHash = value;
        }
    }
}
