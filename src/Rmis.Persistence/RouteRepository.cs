using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rmis.Domain;
using Rmis.Persistence.Abstract;

namespace Rmis.Persistence
{
    public class RouteRepository : EfRepository<Route>, IRouteRepository
    {
        public RouteRepository(DbContext context): base(context, false)
        {
        }

        public IQueryable<Route> GetAll()
        {
            return this.Include(r => r.Direction)
                .IncludeNative(r => r.Stops)
                .ThenInclude(s => s.Station);
        }
    }
}