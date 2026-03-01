using Backend.Api.Infrastructure.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.Health.Queries.GetMinioHealth;

public class GetMinioHealthQueryHandler : IRequestHandler<GetMinioHealthQuery, GetMinioHealthResponse>
{
    private readonly IMinioHealthService _minioHealthService;
    private readonly ILogger<GetMinioHealthQueryHandler> _logger;

    public GetMinioHealthQueryHandler(IMinioHealthService minioHealthService, ILogger<GetMinioHealthQueryHandler> logger)
    {
        _minioHealthService = minioHealthService;
        _logger = logger;
    }

    public async Task<GetMinioHealthResponse> Handle(GetMinioHealthQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetMinioHealthQueryHandler: pinging MinIO endpoint");
        var canConnect = await _minioHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetMinioHealthResponse(true, "MinIO is reachable", DateTime.UtcNow)
            : new GetMinioHealthResponse(false, "MinIO health check failed", DateTime.UtcNow);
    }
}
