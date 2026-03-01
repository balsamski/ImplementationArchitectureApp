using System.IO.Compression;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace Worker.Application.File.Commands.ZipFileFromQueueLocation;

public class ZipFileFromQueueLocationCommandHandler : IRequestHandler<ZipFileFromQueueLocationCommand>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ZipFileFromQueueLocationCommandHandler> _logger;

    public ZipFileFromQueueLocationCommandHandler(
        IConfiguration configuration,
        ILogger<ZipFileFromQueueLocationCommandHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Handle(ZipFileFromQueueLocationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FileLocation))
        {
            _logger.LogWarning("Queue message is empty.");
            return;
        }

        var endpoint = _configuration["Minio:Endpoint"];
        var accessKey = _configuration["Minio:RootUser"];
        var secretKey = _configuration["Minio:RootPassword"];
        var fallbackBucket = _configuration["Minio:BucketName"] ?? "app-files";

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
        {
            _logger.LogError("Missing MinIO configuration values.");
            return;
        }

        var location = request.FileLocation.Trim();
        var slashIndex = location.IndexOf('/');
        var bucket = slashIndex > 0 ? location[..slashIndex] : fallbackBucket;
        var objectName = slashIndex > 0 ? location[(slashIndex + 1)..] : location;

        if (string.IsNullOrWhiteSpace(objectName))
        {
            _logger.LogWarning("Queue message does not include object name: {Location}", location);
            return;
        }

        var endpointUri = new Uri(endpoint);
        var minioClient = new MinioClient()
            .WithEndpoint(endpointUri.Host, endpointUri.Port)
            .WithSSL(endpointUri.Scheme == "https")
            .WithCredentials(accessKey, secretKey)
            .Build();

        await using var sourceStream = new MemoryStream();
        await minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream(async stream => await stream.CopyToAsync(sourceStream, cancellationToken)));

        sourceStream.Position = 0;

        var zipObjectName = Path.ChangeExtension(objectName, ".zip");
        await using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entryName = Path.GetFileName(objectName);
            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            await using var entryStream = entry.Open();
            await sourceStream.CopyToAsync(entryStream, cancellationToken);
        }

        zipStream.Position = 0;

        await minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(zipObjectName)
            .WithStreamData(zipStream)
            .WithObjectSize(zipStream.Length)
            .WithContentType("application/zip"), cancellationToken);

        _logger.LogInformation("Created zip file in MinIO: {Location}", $"{bucket}/{zipObjectName}");
    }
}
