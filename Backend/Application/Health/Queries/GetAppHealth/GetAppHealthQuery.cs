using MediatR;

namespace Backend.Api.Application.Health.Queries.GetAppHealth;

public record GetAppHealthQuery : IRequest<GetAppHealthResponse>;

public record GetAppHealthResponse(bool IsHealthy, string Message, DateTime UtcNow);
