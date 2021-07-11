using System;
using System.Collections.Generic;

namespace Rmis.Google.Sheets.Abstract
{
    public interface IGoogleSheetsScheduleProvider
    {
        IEnumerable<GoogleSchedule> GetSchedules(DateTime fromDate);
    }
}