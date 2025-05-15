namespace api.Models
{
    public interface IUser
    {
        int Id { get; set; }
        string Email { get; set; }
        string PasswordHash { get; set; }
    }
}
