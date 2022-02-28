using System.Threading.Tasks;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure
{
    public interface IDelayFirstFilter<in TMessageEntity> where TMessageEntity : class
    {
        Task<bool> IsFiltered(TMessageEntity message);
    }
}