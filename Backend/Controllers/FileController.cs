using Backend.Api.Application.File.Commands.UploadFile;
using Backend.Api.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FileController> _logger;
    private readonly IFileMetadataService _metadataService;

    public FileController(IMediator mediator, ILogger<FileController> logger, IFileMetadataService metadataService)
    {
        _mediator = mediator;
        _logger = logger;
        _metadataService = metadataService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] UploadFileCommand command)
    {
        _logger.LogInformation("FileController: uploading file {FileName}", command.FileName);
        try
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("FileController: upload for {FileName} succeeded", command.FileName);
            return Ok(result);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "FileController: invalid base64 for {FileName}", command.FileName);
            return BadRequest("Invalid base64 payload");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "FileController: bad request for {FileName}", command.FileName);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUploadedFiles(CancellationToken cancellationToken)
    {
        var files = await _metadataService.GetUploadedFilesAsync(cancellationToken);
        return Ok(files);
    }
}
