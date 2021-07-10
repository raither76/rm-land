using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IRmisDbContext : IUnitOfWork
    {
        IScheduleRepository ScheduleRepository { get; }
        IRmisRepository<Route> RouteRepository { get; }
        IRmisRepository<Station> StationRepository { get; }
        IDirectionRepository DirectionRepository { get; }
    }
}