using Backend.Api.Infrastructure.Redis;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.Health.Queries.GetRedisHealth;

public class GetRedisHealthQueryHandler : IRequestHandler<GetRedisHealthQuery, GetRedisHealthResponse>
{
    private readonly IRedisHealthService _redisHealthService;
    private readonly ILogger<GetRedisHealthQueryHandler> _logger;

    public GetRedisHealthQueryHandler(IRedisHealthService redisHealthService, ILogger<GetRedisHealthQueryHandler> logger)
    {
        _redisHealthService = redisHealthService;
        _logger = logger;
    }

    public async Task<GetRedisHealthResponse> Handle(GetRedisHealthQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRedisHealthQueryHandler: checking redis connection");
        var canConnect = await _redisHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetRedisHealthResponse(true, "Redis connection is healthy", DateTime.UtcNow)
            : new GetRedisHealthResponse(false, "Redis connection failed", DateTime.UtcNow);
    }
}
