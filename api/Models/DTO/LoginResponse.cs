namespace api.Models.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int ResidentId { get; set; }
        public string Resident { get; set; } = string.Empty;
    }
}
