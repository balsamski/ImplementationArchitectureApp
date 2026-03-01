using Backend.Api.Infrastructure.Storage;
using MediatR;

namespace Backend.Api.Application.Health.Queries.GetMinioHealth;

public class GetMinioHealthQueryHandler : IRequestHandler<GetMinioHealthQuery, GetMinioHealthResponse>
{
    private readonly IMinioHealthService _minioHealthService;

    public GetMinioHealthQueryHandler(IMinioHealthService minioHealthService)
    {
        _minioHealthService = minioHealthService;
    }

    public async Task<GetMinioHealthResponse> Handle(GetMinioHealthQuery request, CancellationToken cancellationToken)
    {
        var canConnect = await _minioHealthService.CanConnectAsync(cancellationToken);

        return canConnect
            ? new GetMinioHealthResponse(true, "MinIO is reachable", DateTime.UtcNow)
            : new GetMinioHealthResponse(false, "MinIO health check failed", DateTime.UtcNow);
    }
}
