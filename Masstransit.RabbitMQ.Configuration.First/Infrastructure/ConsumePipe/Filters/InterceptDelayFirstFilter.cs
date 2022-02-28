using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters
{
    public class InterceptDelayFirstFilter<T> : IFilter<ConsumeContext<T>>
        where T : class
    {
        private readonly Type _delayFirstFilterType;

        private readonly ILogger<InterceptDelayFirstFilter<T>> _logger;

        private readonly IServiceProvider _serviceProvider;

        public InterceptDelayFirstFilter(Type delayFirstFilterType, IServiceProvider serviceProvider)
        {
            _delayFirstFilterType = delayFirstFilterType;

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILogger<InterceptDelayFirstFilter<T>>>();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(InterceptDelayFirstFilter<T>));
            scope.Add("type", "delay first filter");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            if (await IsFiltered(context))
            {
                await context.NotifyConsumed(context, context.ReceiveContext.ElapsedTime,
                    TypeMetadataCache<InterceptDelayFirstFilter<T>>.ShortName).ConfigureAwait(false);

                _logger.LogInformation(
                    $"[{nameof(InterceptDelayFirstFilter<T>)}][{nameof(Send)}] Message is filtered by {_delayFirstFilterType.Name}");

                return;
            }

            await next.Send(context);
        }

        private async Task<bool> IsFiltered(ConsumeContext<T> context)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var delayFirstFilter =
                        scope.ServiceProvider.GetRequiredService(_delayFirstFilterType) as
                            IDelayFirstFilter<T>;

                    return await delayFirstFilter.IsFiltered(context.Message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"[{nameof(InterceptDelayFirstFilter<T>)}][{nameof(IsFiltered)} Failed to apply {_delayFirstFilterType.Name} to filter message");

                throw;
            }
        }
    }
}