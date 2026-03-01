using MediatR;

namespace Backend.Api.Application.Health.Queries.GetRabbitMqHealth;

public record GetRabbitMqHealthQuery : IRequest<GetRabbitMqHealthResponse>;

public record GetRabbitMqHealthResponse(bool IsHealthy, string Message, DateTime UtcNow);
