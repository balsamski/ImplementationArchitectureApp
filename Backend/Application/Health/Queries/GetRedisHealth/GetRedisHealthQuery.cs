using MediatR;

namespace Backend.Api.Application.Health.Queries.GetRedisHealth;

public record GetRedisHealthQuery : IRequest<GetRedisHealthResponse>;

public record GetRedisHealthResponse(bool IsHealthy, string Message, DateTime UtcNow);
