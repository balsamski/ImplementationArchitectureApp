using System;
using System.IO;
using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Infrastructure.Storage;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucket;
    private readonly ILogger<MinioFileStorageService> _logger;

    public MinioFileStorageService(IConfiguration configuration, ILogger<MinioFileStorageService> logger)
    {
        _logger = logger;
        _bucket = configuration["Minio:BucketName"] ?? "app-files";
        var endpoint = configuration["Minio:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            _logger.LogError("MinioFileStorageService: missing MinIO endpoint");
            throw new InvalidOperationException("MinIO endpoint configuration is missing.");
        }
        var endpointUri = new Uri(endpoint);

        var accessKey = configuration["Minio:RootUser"];
        var secretKey = configuration["Minio:RootPassword"];
        if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
        {
            _logger.LogError("MinioFileStorageService: missing MinIO credentials");
            throw new InvalidOperationException("MinIO credentials are missing.");
        }

        _minioClient = new MinioClient()
            .WithEndpoint(endpointUri.Host, endpointUri.Port)
            .WithSSL(endpointUri.Scheme == "https")
            .WithCredentials(accessKey!, secretKey!)
            .Build();
    }

    public async Task UploadAsync(string fileName, byte[] data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name must be provided.", nameof(fileName));
        }

        await EnsureBucketExistsAsync(cancellationToken);

        await using var stream = new MemoryStream(data);
        await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("application/octet-stream"),
            cancellationToken);

        _logger.LogInformation("MinioFileStorageService: uploaded {FileName} to bucket {Bucket}", fileName, _bucket);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_bucket);
        if (!await _minioClient.BucketExistsAsync(existsArgs, cancellationToken))
        {
            _logger.LogInformation("MinioFileStorageService: creating bucket {Bucket}", _bucket);
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket), cancellationToken);
        }
    }
}
