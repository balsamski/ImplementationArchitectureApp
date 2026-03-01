namespace Backend.Api.Infrastructure.Database;

public interface IDatabaseHealthService
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
