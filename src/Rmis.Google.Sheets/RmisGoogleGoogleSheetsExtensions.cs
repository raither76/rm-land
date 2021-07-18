using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rmis.Google.Sheets.Abstract;

namespace Rmis.Google.Sheets
{
    public static class RmisGoogleGoogleSheetsExtensions
    {
        public static IServiceCollection AddRmisGoogleGoogleSheets(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("GoogleSheets");
            GoogleSheetsConfig config = new GoogleSheetsConfig
            {
                SheetId = section["SheetId"],
                CredentialFileName = section["CredentialFileName"] 
            };
            
            return services
                .AddSingleton<GoogleSheetsConfig>(config)
                .AddSingleton<IGoogleSheetsScheduleProvider, GoogleSheetsScheduleProvider>();
        }
    }
}