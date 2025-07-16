using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DadJokeConsoleApi.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _database;

        public async Task RemoveKeyAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public RedisCacheService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("Redis")["ConnectionString"];
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
        }

        // Get from cache
        public async Task<string?> GetValueAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        // Set in cache
        public async Task SetValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry ?? TimeSpan.FromMinutes(5));
        }
    }
}
