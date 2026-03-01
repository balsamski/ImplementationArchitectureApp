using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Infrastructure.Database;

public class PostgresFileMetadataService : IFileMetadataService
{
    private readonly FileMetadataDbContext _context;

    public PostgresFileMetadataService(FileMetadataDbContext context)
    {
        _context = context;
    }

    public async Task LogFileUploadAsync(string fileName, string location, DateTime uploadDate, CancellationToken cancellationToken)
    {
        var record = new FileRecord
        {
            FileName = fileName,
            UploadDate = uploadDate,
            Location = location
        };

        _context.Files.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<FileMetadataDto>> GetUploadedFilesAsync(CancellationToken cancellationToken)
    {
        return await _context.Files
            .AsNoTracking()
            .OrderByDescending(f => f.UploadDate)
            .Select(f => new FileMetadataDto(f.Id, f.FileName, f.UploadDate, f.Location))
            .ToListAsync(cancellationToken);
    }
}
