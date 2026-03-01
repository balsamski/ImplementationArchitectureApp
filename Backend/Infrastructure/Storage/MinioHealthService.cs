using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Backend.Api.Infrastructure.Storage;

public class MinioHealthService : IMinioHealthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MinioHealthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        var endpoint = _configuration["Minio:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new Exception("Missing Endpoint string to MinIO");

        var requestUri = BuildHealthUri(endpoint);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await _httpClient.SendAsync(request, cts.Token);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static Uri BuildHealthUri(string baseEndpoint)
    {
        var baseUri = baseEndpoint.EndsWith("/") ? baseEndpoint.TrimEnd('/') : baseEndpoint;
        return new Uri($"{baseUri}/minio/health/live");
    }
}
