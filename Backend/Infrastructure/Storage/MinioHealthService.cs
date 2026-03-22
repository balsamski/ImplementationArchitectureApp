using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Infrastructure.Storage;

public class MinioHealthService : IMinioHealthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MinioHealthService> _logger;

    public MinioHealthService(HttpClient httpClient, IConfiguration configuration, ILogger<MinioHealthService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        var endpoint = _configuration["Minio:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new Exception("Missing Endpoint string to MinIO");

        _logger.LogInformation("MinioHealthService: checking endpoint {Endpoint}", endpoint);
        var requestUri = BuildHealthUri(endpoint);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await _httpClient.SendAsync(request, cts.Token);
            _logger.LogInformation("MinioHealthService: health check returned {StatusCode}", response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MinioHealthService: failed to reach MinIO endpoint");
            return false;
        }
    }

    public string? GetHostIp()
    {
        try
        {
            var endpoint = _configuration["Minio:Endpoint"];
            if (string.IsNullOrWhiteSpace(endpoint))
                return null;

            return endpoint;
        }
        catch
        {
            return null;
        }
    }

    private static Uri BuildHealthUri(string baseEndpoint)
    {
        var baseUri = baseEndpoint.EndsWith("/") ? baseEndpoint.TrimEnd('/') : baseEndpoint;
        return new Uri($"{baseUri}/minio/health/live");
    }
}
