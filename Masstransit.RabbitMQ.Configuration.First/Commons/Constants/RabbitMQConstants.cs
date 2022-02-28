namespace Masstransit.RabbitMQ.Configuration.First.Commons.Constants
{
    public class QueueArguments
    {
        public const string DeadLetterExchange = "x-dead-letter-exchange";
        public const string MessageExpiration = "x-message-ttl";
    }

    public class MessageHeaders
    {
        public const string CustomRedeliveryCount = "Custom-Redelivery-Count";
    }

    public class QueueFormats
    {
        public const string DelayQueue = "{0}_delay";
        public const string InterceptorQueue = "Delay{0}For{1}";
        public const string DelayFirstQueue = "{0}_delay_first";
    }
}