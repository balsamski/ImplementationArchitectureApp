namespace Backend.Api.Infrastructure.RabbitMq;

public interface IFileLocationPublisher
{
    Task PublishAsync(string fileLocation, CancellationToken cancellationToken);
}
