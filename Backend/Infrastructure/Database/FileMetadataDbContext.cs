using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Infrastructure.Database;

public sealed class FileMetadataDbContext : DbContext
{
    public FileMetadataDbContext(DbContextOptions<FileMetadataDbContext> options)
        : base(options)
    {
    }

    public DbSet<FileRecord> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileRecord>(entity =>
        {
            entity.ToTable("files");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasColumnName("file_name");

            entity.Property(e => e.UploadDate)
                .IsRequired()
                .HasColumnName("upload_date");

            entity.Property(e => e.Location)
                .IsRequired()
                .HasColumnName("location");
        });
    }
}
