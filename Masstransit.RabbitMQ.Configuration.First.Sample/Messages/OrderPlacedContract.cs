namespace Masstransit.RabbitMQ.Configuration.First.Sample.Messages
{
    public class OrderPlacedContract
    {
        public int OrderNumber { get; set; }

        public bool IsPaid { get; set; }
    }
}