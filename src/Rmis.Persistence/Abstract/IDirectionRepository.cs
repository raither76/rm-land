using System.Linq;
using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IDirectionRepository : IRmisRepository<Direction>
    {
        IQueryable<Direction> GetAll();
    }
}