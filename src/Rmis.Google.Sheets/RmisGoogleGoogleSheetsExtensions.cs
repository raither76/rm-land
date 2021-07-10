using System;
using Microsoft.Extensions.DependencyInjection;
using Rmis.Google.Sheets.Abstract;

namespace Rmis.Google.Sheets
{
    public static class RmisGoogleGoogleSheetsExtensions
    {
        public static IServiceCollection AddRmisGoogleGoogleSheets(this IServiceCollection services)
        {
            return services.AddSingleton<IGoogleSheetsScheduleProvider, GoogleSheetsScheduleProvider>();
        }
    }
}