using MediatR;

namespace Worker.Application.File.Commands.ZipFileFromQueueLocation;

public record ZipFileFromQueueLocationCommand(string FileLocation) : IRequest;
