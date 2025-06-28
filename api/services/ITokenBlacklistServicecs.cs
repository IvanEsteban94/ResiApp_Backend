namespace api.services
{
    public interface ITokenBlacklistServicecs
    {
        void RevokeToken(string token);
        bool IsTokenRevoked(string token);
    }

}
