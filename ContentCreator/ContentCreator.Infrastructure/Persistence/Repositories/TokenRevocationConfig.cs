using ContentCreator.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class TokenRevocationConfig : ITokenRevocationConfig
    {
        private readonly IDistributedCache _cache;
        public TokenRevocationConfig(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<bool> RevokeToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };

            await _cache.SetStringAsync(token, "revoked", options);

            return true;
        }

        public async Task<bool> IsTokenRevoked(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var value = await _cache.GetStringAsync(token);
            return value == "revoked";
        }
    }
}