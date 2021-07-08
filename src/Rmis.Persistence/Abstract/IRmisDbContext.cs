using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IRmisDbContext
    {
        IRmisRepository<Schedule> ScheduleRepository { get; }
        IRmisRepository<Route> RouteRepository { get; }
        IRmisRepository<Station> StationRepository { get; }
    }
}