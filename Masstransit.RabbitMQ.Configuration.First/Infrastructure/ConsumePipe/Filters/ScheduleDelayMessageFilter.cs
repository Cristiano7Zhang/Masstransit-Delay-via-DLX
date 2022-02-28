using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Commons.Constants;
using MassTransit.RabbitMqTransport.Contexts;
using MassTransit.Transports.InMemory;
using RabbitMQ.Client;
using MessageHeaders = Masstransit.RabbitMQ.Configuration.First.Commons.Constants.MessageHeaders;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters
{
    public class ScheduleDelayMessageFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        private readonly int _redeliveryLimit;

        public ScheduleDelayMessageFilter(int redeliveryLimit)
        {
            _redeliveryLimit = redeliveryLimit;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(ScheduleDelayMessageFilter<T>));
            scope.Add("type", "schedule delay message");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (OperationCanceledException exception)
                when (exception.CancellationToken == context.CancellationToken)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!CanScheduleDelayMessage(context)) throw;

                try
                {
                    await ScheduleDelayMessage(context);
                }
                catch (Exception redeliveryException)
                {
                    throw new TransportException(context.ReceiveContext.InputAddress,
                        "The message delivery could not be rescheduled",
                        new AggregateException(redeliveryException, exception));
                }
            }
        }

        private bool CanScheduleDelayMessage(ConsumeContext<T> context)
        {
            var redeliveryCount = context.Headers.Get(MessageHeaders.CustomRedeliveryCount, default(int?)) ?? 0;

            return redeliveryCount < _redeliveryLimit;
        }

        private async Task ScheduleDelayMessage(ConsumeContext<T> context)
        {
            var redeliveryCount = context.Headers.Get(MessageHeaders.CustomRedeliveryCount, default(int?)) ?? 0;

            // Choose SendEnpoint instead of PublishEndpoint here, since customizing destination exchange is not supported for PublishEndpoint
            // See more details by https://masstransit-project.com/advanced/topology/rabbitmq.html#exchanges
            var sendEndpoint = await context.GetSendEndpoint(GetDelayAddress(context));

            await sendEndpoint.Send(context.Message,
                sendContext => { sendContext.Headers.Set(MessageHeaders.CustomRedeliveryCount, redeliveryCount + 1); });
        }

        private Uri GetDelayAddress(ConsumeContext<T> context)
        {
            var receiveContext = (RabbitMqReceiveContext)context.ReceiveContext;

            var delayQueue = string.Format(QueueFormats.DelayQueue,
                receiveContext.InputAddress.GetQueueOrExchangeName());

            return new Uri($"exchange:{delayQueue}?type={ExchangeType.Direct}");
        }
    }
}