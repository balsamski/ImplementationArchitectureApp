using MediatR;

namespace Backend.Api.Application.Health.Queries.GetMinioHealth;

public record GetMinioHealthQuery : IRequest<GetMinioHealthResponse>;

public record GetMinioHealthResponse(bool IsHealthy, string Message, string? HostIp, DateTime UtcNow);
