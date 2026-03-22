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
        var hostIp = _minioHealthService.GetHostIp();

        var successMessage = $"MinIO is reachable, MinIO Server: {hostIp}";
        var failureMessage = $"MinIO health check failed, MinIO Server: {hostIp}";

        return canConnect
            ? new GetMinioHealthResponse(true, successMessage, hostIp, DateTime.UtcNow)
            : new GetMinioHealthResponse(false, failureMessage, hostIp, DateTime.UtcNow);
    }
}
