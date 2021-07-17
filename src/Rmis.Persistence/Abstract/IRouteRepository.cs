using System.Linq;
using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IRouteRepository : IRmisRepository<Route>
    {
        IQueryable<Route> GetAll();
    }
}