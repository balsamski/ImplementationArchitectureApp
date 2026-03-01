namespace Backend.Api.Infrastructure.Database;

public sealed class FileRecord
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    public string Location { get; set; } = null!;
}
