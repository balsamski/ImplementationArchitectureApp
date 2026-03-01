namespace Backend.Api.Infrastructure.Database;

public sealed record FileMetadataDto(int Id, string FileName, DateTime UploadDate, string Location);
