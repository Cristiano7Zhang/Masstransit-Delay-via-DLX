using System;
using System.Threading.Tasks;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Sample.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Consumers
{
    public class HelloMessageConsumer : IConsumer<HelloMessageContract>
    {
        private readonly ILogger<HelloMessageConsumer> _logger;

        public HelloMessageConsumer(ILogger<HelloMessageConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<HelloMessageContract> context)
        {
            try
            {
                _logger.LogInformation($"Receiving hello message: {JsonConvert.SerializeObject(context.Message)}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[{nameof(HelloMessageConsumer)}][{nameof(Consume)}] Failed to do ...");

                throw;
            }
        }
    }
}