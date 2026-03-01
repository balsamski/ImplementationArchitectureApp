namespace Backend.Api.Infrastructure.RabbitMq;

public interface IRabbitMqHealthService
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
