using api.Interface;
using System.Collections.Concurrent;

namespace api.services
{
   

        public class InMemoryTokenBlacklistService : ITokenBlacklistService
        {
            private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

            public Task AddAsync(string token, DateTime expires)
            {
                _blacklist[token] = expires;
                return Task.CompletedTask;
            }

            public Task<bool> IsTokenRevokedAsync(string token)
            {
                if (_blacklist.TryGetValue(token, out var expiration))
                {
                    if (expiration > DateTime.UtcNow)
                        return Task.FromResult(true);
                    _blacklist.TryRemove(token, out _);
                }

                return Task.FromResult(false);
            }
        }
    }

