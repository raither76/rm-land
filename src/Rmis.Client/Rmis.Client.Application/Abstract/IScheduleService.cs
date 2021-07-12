using System.Collections.Generic;
using Rmis.Client.Domain;

namespace Rmis.Client.Application.Abstract
{
    public interface IScheduleService
    {
        IEnumerable<Schedule> GetSchedules();

        void LoadSchedules();
    }
}