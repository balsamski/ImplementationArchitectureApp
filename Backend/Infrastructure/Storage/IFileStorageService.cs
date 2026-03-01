namespace Backend.Api.Infrastructure.Storage;

public interface IFileStorageService
{
    Task UploadAsync(string fileName, byte[] data, CancellationToken cancellationToken);
}
