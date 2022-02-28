using System;
using System.Reflection;
using GreenPipes.Internals.Extensions;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Commons.Helpers;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Observers;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe
{
    public static class ConsumePipeConfiguratorExtensions
    {
        public static void UseScheduleDelayMessageFilter(this IConsumePipeConfigurator configurator,
            int redeliveryLimit)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (redeliveryLimit <= 0)
                throw new ArgumentException("RedeliveryLimit must be more than 0");

            var observer = new ScheduleDelayMessageFilterConfigurationObserver(configurator, redeliveryLimit);
        }

        public static void UseInterceptDelayFirstFilter(this IConsumePipeConfigurator configurator,
            string delayFirstFilterType, IServiceProvider serviceProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var filterType = TypeHelper.GetType(Assembly.GetEntryAssembly(), delayFirstFilterType);
            if (filterType == null || !filterType.HasInterface(typeof(IDelayFirstFilter<>)))
                throw new ArgumentException(
                    $"{delayFirstFilterType} is not found or doesn't implement {typeof(IDelayFirstFilter<>).Name}");

            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var observer =
                new InterceptDelayFirstFilterConfigurationObserver(configurator, filterType, serviceProvider);
        }
    }
}