namespace Backend.Api.Infrastructure.Storage;

public interface IMinioHealthService
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
