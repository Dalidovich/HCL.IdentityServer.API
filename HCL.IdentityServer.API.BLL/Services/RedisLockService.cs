using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class RedisLockService : IRedisLockService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisOptions _redisOptions;

        public RedisLockService(IDistributedCache distributedCache, RedisOptions redisOptions)
        {
            _distributedCache = distributedCache;
            _redisOptions = redisOptions;
        }

        public async Task SetString(AthorPublicProfileReply reply, string id)
        {
            var existingConnectionMultiplexer = ConnectionMultiplexer.Connect(_redisOptions.Host);
            var multiplexers = new List<RedLockMultiplexer>
            {
                existingConnectionMultiplexer
            };
            var redlockFactory = RedLockFactory.Create(multiplexers);

            await using (var redLock = await redlockFactory.CreateLockAsync
                (_redisOptions.Resource, _redisOptions.Expiry, _redisOptions.Wait, _redisOptions.Retry))
            {
                if (redLock.IsAcquired)
                {
                    var accountString = JsonSerializer.Serialize(reply);
                    await _distributedCache.SetStringAsync(id, accountString, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _redisOptions.StoreDuration
                    });
                }
            }
        }
    }
}
