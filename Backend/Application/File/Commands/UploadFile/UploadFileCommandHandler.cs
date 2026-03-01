using Backend.Api.Infrastructure.Storage;
using Backend.Api.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.File.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private const string FilesCacheKey = "files:all";

    private readonly IFileStorageService _storageService;
    private readonly IFileMetadataService _metadataService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UploadFileCommandHandler> _logger;
    private readonly string _bucketName;

    public UploadFileCommandHandler(
        IFileStorageService storageService,
        IFileMetadataService metadataService,
        IDistributedCache cache,
        ILogger<UploadFileCommandHandler> logger,
        IConfiguration configuration)
    {
        _storageService = storageService;
        _metadataService = metadataService;
        _cache = cache;
        _logger = logger;
        _bucketName = configuration["Minio:BucketName"] ?? "app-files";
    }

    public async Task<UploadFileResult> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UploadFileCommandHandler: storing {FileName}", request.FileName);
        if (string.IsNullOrWhiteSpace(request.Base64Content))
        {
            _logger.LogWarning("UploadFileCommandHandler: base64 payload is missing for {FileName}", request.FileName);
            throw new ArgumentException("Base64 content is required", nameof(request.Base64Content));
        }

        byte[] data;
        try
        {
            data = Convert.FromBase64String(request.Base64Content);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "UploadFileCommandHandler: invalid base64 for {FileName}", request.FileName);
            throw;
        }

        var uploadDate = DateTime.UtcNow;
        var fileNameWithUploadDate = BuildFileNameWithUploadDate(request.FileName, uploadDate);

        await _storageService.UploadAsync(fileNameWithUploadDate, data, cancellationToken);
        await _metadataService.LogFileUploadAsync(
            fileNameWithUploadDate,
            $"{_bucketName}/{fileNameWithUploadDate}",
            uploadDate,
            cancellationToken);
        await _cache.RemoveAsync(FilesCacheKey, cancellationToken);
        _logger.LogInformation("UploadFileCommandHandler: stored {FileName} ({Bytes} bytes) in {Bucket}", fileNameWithUploadDate, data.Length, _bucketName);
        return new UploadFileResult(_bucketName, fileNameWithUploadDate, data.Length);
    }

    private static string BuildFileNameWithUploadDate(string fileName, DateTime uploadDate)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var timestamp = uploadDate.ToString("yyyyMMddHHmmss");
        return $"{nameWithoutExtension}_{timestamp}{extension}";
    }
}
