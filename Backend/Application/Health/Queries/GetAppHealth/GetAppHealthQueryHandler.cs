using MediatR;

namespace Backend.Api.Application.Health.Queries.GetAppHealth;

public class GetAppHealthQueryHandler : IRequestHandler<GetAppHealthQuery, GetAppHealthResponse>
{
    public Task<GetAppHealthResponse> Handle(GetAppHealthQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAppHealthResponse(
            IsHealthy: true,
            Message: "Application is running",
            UtcNow: DateTime.UtcNow);

        return Task.FromResult(response);
    }
}
