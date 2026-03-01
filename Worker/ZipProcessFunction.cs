using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Worker.Application.File.Commands.ZipFileFromQueueLocation;

namespace Worker
{
    public class ZipProcessFunction
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ZipProcessFunction(ILoggerFactory loggerFactory, IMediator mediator)
        {
            _logger = loggerFactory.CreateLogger<ZipProcessFunction>();
            _mediator = mediator;
        }

        [Function("ZipProcessFunction")]
        public async Task Run([RabbitMQTrigger("file-locations", ConnectionStringSetting = "RabbitMqConnection")] string myQueueItem)
        {
            _logger.LogInformation("Received file location from queue: {Location}", myQueueItem);
            await _mediator.Send(new ZipFileFromQueueLocationCommand(myQueueItem));
        }
    }
}
