using Backend.Api.Infrastructure.Redis;
using MediatR;

namespace Backend.Api.Application.Health.Queries.GetRedisHealth;

public class GetRedisHealthQueryHandler : IRequestHandler<GetRedisHealthQuery, GetRedisHealthResponse>
{
    private readonly IRedisHealthService _redisHealthService;

    public GetRedisHealthQueryHandler(IRedisHealthService redisHealthService)
    {
        _redisHealthService = redisHealthService;
    }

    public async Task<GetRedisHealthResponse> Handle(GetRedisHealthQuery request, CancellationToken cancellationToken)
    {
        var canConnect = await _redisHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetRedisHealthResponse(true, "Redis connection is healthy", DateTime.UtcNow)
            : new GetRedisHealthResponse(false, "Redis connection failed", DateTime.UtcNow);
    }
}
