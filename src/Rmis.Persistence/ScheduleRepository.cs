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
                .Where(s => s.Route.Direction.Id == directionId && !s.IsSynchronized && s.Date >= fromDate);
        }
        
        public IQueryable<Schedule> GetAllByFromDate(DateTime fromDate)
        {
            return this.Include(s => s.Route)
                .Include(s => s.Route.Direction)
                .Where(s => s.Date >= fromDate);
        }

        public IQueryable<Schedule> GetActualAllByTrainNumber(string trainNumber)
        {
            return this.Include(s => s.Route)
                .Include(s => s.Route.Direction)
                .Include(s => s.Route.Direction.FromStation)
                .Include(s => s.Route.Direction.ToStation)
                .Where(s => s.TrainNumber == trainNumber && s.Date >= DateTime.Now.Date);
        }

        public int RemoveBeforeDate(DateTime beforeDate)
        {
            return this.Delete(s => s.Date < beforeDate);
        }
    }
}