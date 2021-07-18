using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IRmisDbContext : IUnitOfWork
    {
        IScheduleRepository ScheduleRepository { get; }
        IRouteRepository RouteRepository { get; }
        IRmisRepository<Station> StationRepository { get; }
        IDirectionRepository DirectionRepository { get; }
        IRmisRepository<Stop> StopRepository { get; }
        IRmisRepository<TrackInfo> TrackInfoRepository { get; }
    }
}