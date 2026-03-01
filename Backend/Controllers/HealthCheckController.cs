using Backend.Api.Application.Health.Queries.GetAppHealth;
using Backend.Api.Application.Health.Queries.GetDatabaseHealth;
using Backend.Api.Application.Health.Queries.GetMinioHealth;
using Backend.Api.Application.Health.Queries.GetRabbitMqHealth;
using Backend.Api.Application.Health.Queries.GetRedisHealth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthCheckController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("app")]
    public async Task<IActionResult> AppHealth(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppHealthQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("database")]
    public async Task<IActionResult> DatabaseHealth(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDatabaseHealthQuery(), cancellationToken);
        return result.IsHealthy
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }

    [HttpGet("rabbitmq")]
    public async Task<IActionResult> RabbitMqHealth(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRabbitMqHealthQuery(), cancellationToken);
        return result.IsHealthy
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }

    [HttpGet("minio")]
    public async Task<IActionResult> MinioHealth(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMinioHealthQuery(), cancellationToken);
        return result.IsHealthy
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }

    [HttpGet("redis")]
    public async Task<IActionResult> RedisHealth(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRedisHealthQuery(), cancellationToken);
        return result.IsHealthy
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
