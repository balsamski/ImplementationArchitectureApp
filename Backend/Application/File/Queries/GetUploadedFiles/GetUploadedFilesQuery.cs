using Backend.Api.Infrastructure.Database;
using MediatR;

namespace Backend.Api.Application.File.Queries.GetUploadedFiles;

public record GetUploadedFilesQuery : IRequest<List<FileMetadataDto>>;
