using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Backend.Api.Infrastructure.RabbitMq;

public class RabbitMqHealthService : IRabbitMqHealthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqHealthService> _logger;

    public RabbitMqHealthService(IConfiguration configuration, ILogger<RabbitMqHealthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("RabbitMq");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning("RabbitMqHealthService: missing RabbitMq connection string");
            throw new Exception("Missing connection string to RabbitMq");
        }

        try
        {
            _logger.LogInformation("RabbitMqHealthService: attempting to reach {ConnectionString}", connectionString);
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("RabbitMqHealthService: connection open status {IsOpen}", connection.IsOpen);
            return connection.IsOpen;
        }
        catch
        {
            _logger.LogError("RabbitMqHealthService: failed to connect to RabbitMQ");
            return false;
        }
    }
}
