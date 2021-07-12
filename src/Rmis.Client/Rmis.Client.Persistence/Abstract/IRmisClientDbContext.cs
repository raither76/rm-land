using Rmis.Client.Domain;

namespace Rmis.Client.Persistence.Abstract
{
    public interface IRmisClientDbContext : IUnitOfWork
    {
        IRmisClientRepository<Schedule> ScheduleRepository { get; }
    }
}