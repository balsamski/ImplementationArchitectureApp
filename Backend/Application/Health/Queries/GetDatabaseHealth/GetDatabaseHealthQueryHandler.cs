using Backend.Api.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.Health.Queries.GetDatabaseHealth;

public class GetDatabaseHealthQueryHandler : IRequestHandler<GetDatabaseHealthQuery, GetDatabaseHealthResponse>
{
    private readonly IDatabaseHealthService _databaseHealthService;
    private readonly ILogger<GetDatabaseHealthQueryHandler> _logger;

    public GetDatabaseHealthQueryHandler(IDatabaseHealthService databaseHealthService, ILogger<GetDatabaseHealthQueryHandler> logger)
    {
        _databaseHealthService = databaseHealthService;
        _logger = logger;
    }

    public async Task<GetDatabaseHealthResponse> Handle(GetDatabaseHealthQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetDatabaseHealthQueryHandler: starting database health check");
        var canConnect = await _databaseHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetDatabaseHealthResponse(true, "Database connection is healthy", DateTime.UtcNow)
            : new GetDatabaseHealthResponse(false, "Database connection failed", DateTime.UtcNow);
    }
}
