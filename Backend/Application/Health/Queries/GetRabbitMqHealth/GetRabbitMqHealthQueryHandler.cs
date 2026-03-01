using Backend.Api.Infrastructure.RabbitMq;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.Health.Queries.GetRabbitMqHealth;

public class GetRabbitMqHealthQueryHandler : IRequestHandler<GetRabbitMqHealthQuery, GetRabbitMqHealthResponse>
{
    private readonly IRabbitMqHealthService _rabbitMqHealthService;
    private readonly ILogger<GetRabbitMqHealthQueryHandler> _logger;

    public GetRabbitMqHealthQueryHandler(IRabbitMqHealthService rabbitMqHealthService, ILogger<GetRabbitMqHealthQueryHandler> logger)
    {
        _rabbitMqHealthService = rabbitMqHealthService;
        _logger = logger;
    }

    public async Task<GetRabbitMqHealthResponse> Handle(GetRabbitMqHealthQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRabbitMqHealthQueryHandler: checking rabbitmq connection");
        var canConnect = await _rabbitMqHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetRabbitMqHealthResponse(true, "RabbitMQ connection is healthy", DateTime.UtcNow)
            : new GetRabbitMqHealthResponse(false, "RabbitMQ connection failed", DateTime.UtcNow);
    }
}
