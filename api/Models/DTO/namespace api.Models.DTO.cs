namespace api.Models.DTO
{
    public class UserUpdateDto
    {
        public string ResidentName { get; set; }
        public string Email { get; set; }
        public string ApartmentInformation { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" or "Resident"
    }
}
