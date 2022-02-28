using System;
using System.Collections.Generic;
using Masstransit.RabbitMQ.Configuration.First.Commons.Constants;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications;
using RabbitMQ.Client;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.Configurations
{
    public class ConsumerConfiguration
    {
        public ConsumerBaseConfiguration BaseSetting { get; set; }

        public ConsumerAdvancedSettings AdvancedSettings { get; set; } = new ConsumerAdvancedSettings();

        public DeadLetterTopologyConfiguration ScheduleDelayMessageTopology
        {
            get
            {
                var delayQueue = string.Format(QueueFormats.DelayQueue, BaseSetting.Queue);

                var topology = new DeadLetterTopologyConfiguration(delayQueue, BaseSetting.Queue,
                    TimeSpan.FromSeconds(AdvancedSettings.Redelivery.DelaySeconds));

                return topology;
            }
        }

        public DeadLetterTopologyConfiguration DelayFirstTopology
        {
            get
            {
                var delayFirstQueue = string.Format(QueueFormats.DelayFirstQueue, BaseSetting.Queue);

                var topology = new DeadLetterTopologyConfiguration(delayFirstQueue, BaseSetting.Queue,
                    TimeSpan.FromSeconds(AdvancedSettings.DelayFirst.DelaySeconds));

                return topology;
            }
        }
    }

    public class ConsumerBaseConfiguration
    {
        public string Queue { get; set; }

        public ExchangeConfiguration BindExchange { get; set; }

        public int PrefetchCount { get; set; }
    }

    public class ExchangeConfiguration
    {
        public string Name { get; set; }

        public string Type { get; set; } = ExchangeType.Fanout;

        public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();
    }

    public class ConsumerAdvancedSettings
    {
        public DelayFirstConfiguration DelayFirst { get; set; } = new DelayFirstConfiguration();

        public RetryRedeliveryConfiguration Retry { get; set; } = new RetryRedeliveryConfiguration();

        public RetryRedeliveryConfiguration Redelivery { get; set; } = new RetryRedeliveryConfiguration();
    }

    public class DelayFirstConfiguration
    {
        public string Filter { get; set; }

        public int DelaySeconds { get; set; } = 0;
    }

    public class RetryRedeliveryConfiguration
    {
        public int Limit { get; set; } = 0;

        public int DelaySeconds { get; set; } = 0;
    }
}