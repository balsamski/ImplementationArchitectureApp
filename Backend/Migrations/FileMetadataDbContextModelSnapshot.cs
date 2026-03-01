using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Api.Infrastructure.Database.Migrations;

[DbContext(typeof(FileMetadataDbContext))]
partial class FileMetadataDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.0")
            .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        modelBuilder.Entity("Backend.Api.Infrastructure.Database.FileRecord", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            b.Property<string>("FileName")
                .IsRequired()
                .HasColumnName("file_name");

            b.Property<string>("Location")
                .IsRequired()
                .HasColumnName("location");

            b.Property<DateTime>("UploadDate")
                .IsRequired()
                .HasColumnName("upload_date");

            b.HasKey("Id");

            b.ToTable("files");
        });
    }
}
