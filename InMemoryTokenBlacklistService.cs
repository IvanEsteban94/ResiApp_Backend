using api.Interface;
using System.Collections.Concurrent;

namespace api.services
{
    // Services/InMemoryTokenBlacklistService.cs
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly HashSet<string> _revokedTokens = new HashSet<string>();

        public void RevokeToken(string token)
        {
            _revokedTokens.Add(token);
        }

        public bool IsTokenRevoked(string token)
        {
            return _revokedTokens.Contains(token);
        }

        public Task AddAsync(string token, DateTime expires)
        {
            _revokedTokens.Add(token);
            return Task.CompletedTask
        }

        public Task<bool> IsTokenRevokedAsync(string token)
        {
            return Task.FromResult(_revokedTokens.Contains(token)); 
    }
}
