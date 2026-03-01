using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Backend.Api.Infrastructure.Redis;

public class RedisHealthService : IRedisHealthService
{
    private readonly IConfiguration _configuration;

    public RedisHealthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Missing connection string to Redis");

        try
        {
            using var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            var database = connection.GetDatabase();
            var pong = await database.PingAsync();
            return pong.TotalMilliseconds >= 0;
        }
        catch
        {
            return false;
        }
    }
}
