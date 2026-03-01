using MediatR;

namespace Backend.Api.Application.File.Commands.UploadFile;

public record UploadFileCommand(string FileName, string Base64Content) : IRequest<UploadFileResult>;

public record UploadFileResult(string Bucket, string FileName, long Size);
