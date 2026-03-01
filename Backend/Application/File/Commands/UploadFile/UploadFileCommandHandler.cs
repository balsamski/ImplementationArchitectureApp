using Backend.Api.Infrastructure.Storage;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Application.File.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly IFileStorageService _storageService;
    private readonly ILogger<UploadFileCommandHandler> _logger;
    private readonly string _bucketName;

    public UploadFileCommandHandler(IFileStorageService storageService, ILogger<UploadFileCommandHandler> logger, IConfiguration configuration)
    {
        _storageService = storageService;
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

        await _storageService.UploadAsync(request.FileName, data, cancellationToken);
        _logger.LogInformation("UploadFileCommandHandler: stored {FileName} ({Bytes} bytes) in {Bucket}", request.FileName, data.Length, _bucketName);
        return new UploadFileResult(_bucketName, request.FileName, data.Length);
    }
}
