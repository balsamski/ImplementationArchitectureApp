namespace Backend.Api.Infrastructure.Redis;

public interface IRedisHealthService
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
