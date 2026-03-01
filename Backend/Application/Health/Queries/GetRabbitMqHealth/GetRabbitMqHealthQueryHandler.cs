using Backend.Api.Infrastructure.RabbitMq;
using MediatR;

namespace Backend.Api.Application.Health.Queries.GetRabbitMqHealth;

public class GetRabbitMqHealthQueryHandler : IRequestHandler<GetRabbitMqHealthQuery, GetRabbitMqHealthResponse>
{
    private readonly IRabbitMqHealthService _rabbitMqHealthService;

    public GetRabbitMqHealthQueryHandler(IRabbitMqHealthService rabbitMqHealthService)
    {
        _rabbitMqHealthService = rabbitMqHealthService;
    }

    public async Task<GetRabbitMqHealthResponse> Handle(GetRabbitMqHealthQuery request, CancellationToken cancellationToken)
    {
        var canConnect = await _rabbitMqHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetRabbitMqHealthResponse(true, "RabbitMQ connection is healthy", DateTime.UtcNow)
            : new GetRabbitMqHealthResponse(false, "RabbitMQ connection failed", DateTime.UtcNow);
    }
}
