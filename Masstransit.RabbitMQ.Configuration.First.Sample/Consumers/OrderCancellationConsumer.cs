using System.Threading.Tasks;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Sample.Messages;
using Microsoft.Extensions.Logging;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Consumers
{
    public class OrderCancellationConsumer : IConsumer<OrderPlacedContract>
    {
        private readonly ILogger<OrderCancellationConsumer> _logger;

        public OrderCancellationConsumer(ILogger<OrderCancellationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPlacedContract> context)
        {
            // Cancel order if it is still unpaid
            _logger.LogInformation(
                $"Order: {context.Message.OrderNumber.ToString()} is cancelled as it is still unpaid 30 minutes after placed");
        }
    }
}