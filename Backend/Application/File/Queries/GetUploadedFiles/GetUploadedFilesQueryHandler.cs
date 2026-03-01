using Backend.Api.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Backend.Api.Application.File.Queries.GetUploadedFiles;

public class GetUploadedFilesQueryHandler : IRequestHandler<GetUploadedFilesQuery, List<FileMetadataDto>>
{
    private const string FilesCacheKey = "files:all";

    private readonly IFileMetadataService _metadataService;
    private readonly IDistributedCache _cache;

    public GetUploadedFilesQueryHandler(IFileMetadataService metadataService, IDistributedCache cache)
    {
        _metadataService = metadataService;
        _cache = cache;
    }

    public async Task<List<FileMetadataDto>> Handle(GetUploadedFilesQuery request, CancellationToken cancellationToken)
    {
        var cachedFiles = await _cache.GetStringAsync(FilesCacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedFiles))
        {
            var deserialized = JsonSerializer.Deserialize<List<FileMetadataDto>>(cachedFiles);
            if (deserialized is not null)
            {
                return deserialized;
            }
        }

        var files = await _metadataService.GetUploadedFilesAsync(cancellationToken);

        var serialized = JsonSerializer.Serialize(files);
        await _cache.SetStringAsync(
            FilesCacheKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return files;
    }
}
