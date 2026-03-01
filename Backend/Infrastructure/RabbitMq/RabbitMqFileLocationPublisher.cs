using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Backend.Api.Infrastructure.RabbitMq;

public class RabbitMqFileLocationPublisher : IFileLocationPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqFileLocationPublisher> _logger;

    public RabbitMqFileLocationPublisher(IConfiguration configuration, ILogger<RabbitMqFileLocationPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishAsync(string fileLocation, CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("RabbitMq");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing connection string to RabbitMq");
        }

        var queueName = _configuration["RabbitMq:FileLocationQueue"] ?? "file-locations";

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(fileLocation);
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("RabbitMqFileLocationPublisher: published file location {Location} to queue {Queue}", fileLocation, queueName);
    }
}
