using Backend.Api.Infrastructure.Database;
using MediatR;

namespace Backend.Api.Application.Health.Queries.GetDatabaseHealth;

public class GetDatabaseHealthQueryHandler : IRequestHandler<GetDatabaseHealthQuery, GetDatabaseHealthResponse>
{
    private readonly IDatabaseHealthService _databaseHealthService;

    public GetDatabaseHealthQueryHandler(IDatabaseHealthService databaseHealthService)
    {
        _databaseHealthService = databaseHealthService;
    }

    public async Task<GetDatabaseHealthResponse> Handle(GetDatabaseHealthQuery request, CancellationToken cancellationToken)
    {
        var canConnect = await _databaseHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetDatabaseHealthResponse(true, "Database connection is healthy", DateTime.UtcNow)
            : new GetDatabaseHealthResponse(false, "Database connection failed", DateTime.UtcNow);
    }
}
