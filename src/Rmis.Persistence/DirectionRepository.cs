using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rmis.Domain;
using Rmis.Persistence.Abstract;

namespace Rmis.Persistence
{
    internal class DirectionRepository : EfRepository<Direction>, IDirectionRepository
    {
        internal DirectionRepository(DbContext dbContext) : base(dbContext, false)
        {
        }

        public IQueryable<Direction> GetAll()
        {
            return this.Include(s => s.FromStation)
                .Include(s => s.ToStation);
        }
    }
}