using System;
using System.Linq;
using Rmis.Domain;

namespace Rmis.Persistence.Abstract
{
    public interface IScheduleRepository : IRmisRepository<Schedule>
    {
        IQueryable<Schedule> GetAllByDirectionAndFromDate(long directionId, DateTime fromDate);
        
        IQueryable<Schedule> GetAllByFromDate(DateTime fromDate);

        IQueryable<Schedule> GetActualAllByTrainNumber(string trainNumber);

        int RemoveBeforeDate(DateTime beforeDate);
    }
}