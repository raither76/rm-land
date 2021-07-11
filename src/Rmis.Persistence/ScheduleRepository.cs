using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rmis.Domain;
using Rmis.Persistence.Abstract;
using Z.EntityFramework.Plus;

namespace Rmis.Persistence
{
    internal class ScheduleRepository : EfRepository<Schedule>, IScheduleRepository
    {
        internal ScheduleRepository(DbContext dbContext) : base(dbContext, false)
        {
        }
        
        public IQueryable<Schedule> GetAllByDirectionAndFromDate(long directionId, DateTime fromDate)
        {
            return this.Include(s => s.Route)
                .Include(s => s.Route.Direction)
                .Where(s => s.Route.Direction.Id == directionId && s.Date >= fromDate);
        }
        
        public IQueryable<Schedule> GetAllByFromDate(DateTime fromDate)
        {
            return this.Include(s => s.Route)
                .Include(s => s.Route.Direction)
                .Where(s => s.Date >= fromDate);
        }

        public IQueryable<Schedule> GetAllByRouteNumber(string routeNumber)
        {
            return this.Include(s => s.Route)
                .Include(s => s.Route.Direction)
                .Where(s => s.Route.Number.ToString() == routeNumber);
        }

        public int RemoveBeforeDate(DateTime beforeDate)
        {
            return this.Delete(s => s.Date < beforeDate);
        }
    }
}