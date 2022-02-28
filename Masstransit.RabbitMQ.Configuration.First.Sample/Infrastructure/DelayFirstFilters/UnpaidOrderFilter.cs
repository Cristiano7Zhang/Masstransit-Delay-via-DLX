using System.Threading.Tasks;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure;
using Masstransit.RabbitMQ.Configuration.First.Sample.Messages;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Infrastructure.DelayFirstFilters
{
    public class UnpaidOrderFilter : IDelayFirstFilter<OrderPlacedContract>
    {
        public async Task<bool> IsFiltered(OrderPlacedContract message)
        {
            return message.IsPaid;
        }
    }
}