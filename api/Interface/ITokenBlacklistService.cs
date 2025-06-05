namespace api.Interface
{
    public interface ITokenBlacklistService
    {
        Task AddAsync(string token, DateTime expires);
        Task<bool> IsTokenRevokedAsync(string token);
    }
}
