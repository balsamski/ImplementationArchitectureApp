namespace Backend.Api.Infrastructure.Database;

public interface IFileMetadataService
{
    Task LogFileUploadAsync(string fileName, string location, DateTime uploadDate, CancellationToken cancellationToken);
    Task<List<FileMetadataDto>> GetUploadedFilesAsync(CancellationToken cancellationToken);
}
