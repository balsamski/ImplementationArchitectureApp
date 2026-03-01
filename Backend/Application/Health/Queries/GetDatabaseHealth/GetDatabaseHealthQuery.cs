using MediatR;

namespace Backend.Api.Application.Health.Queries.GetDatabaseHealth;

public record GetDatabaseHealthQuery : IRequest<GetDatabaseHealthResponse>;

public record GetDatabaseHealthResponse(bool IsHealthy, string Message, DateTime UtcNow);
