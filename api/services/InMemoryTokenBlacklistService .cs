using api.Interface;
using System.Collections.Concurrent;

namespace api.services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

        public Task AddAsync(string token, DateTime expires)
        {
            _revokedTokens[token] = expires;
            return Task.CompletedTask;
        }

        public Task<bool> IsTokenRevokedAsync(string token)
        {
            if (_revokedTokens.TryGetValue(token, out var expiration))
            {
                if (DateTime.UtcNow < expiration)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    _revokedTokens.TryRemove(token, out _);
                }
            }
            return Task.FromResult(false);
        }
    }
}
