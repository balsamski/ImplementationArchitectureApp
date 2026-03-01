using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Backend.Api.Infrastructure.RabbitMq;

public class RabbitMqHealthService : IRabbitMqHealthService
{
    private readonly IConfiguration _configuration;

    public RabbitMqHealthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("RabbitMq");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Missing connection string to RabbitMq");

        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            return connection.IsOpen;
        }
        catch
        {
            return false;
        }
    }
}
