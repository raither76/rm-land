using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rmis.Client.Application.Abstract;
using Rmis.Client.Domain;
using Rmis.Client.Persistence.Abstract;

namespace Rmis.Client.Application
{
    internal class ScheduleService : IScheduleService
    {
        private readonly IRmisClientDbContext _context;
        private readonly AppConfig _appConfig;

        internal ScheduleService(IRmisClientDbContext dbContext, AppConfig appConfig)
        {
            _context = dbContext;
            _appConfig = appConfig;
        }
    
        public IEnumerable<Schedule> GetSchedules()
        {
            throw new NotImplementedException();
        }

        public void LoadSchedules()
        {
            try
            {
                using HttpClient client = new HttpClient();
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "routeNumber", _appConfig.RouteNumber }
                };

                string url = QueryHelpers.AddQueryString(_appConfig.RmisHubUrl, parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            
                HttpResponseMessage responseMessage = client.Send(requestMessage);
                responseMessage.EnsureSuccessStatusCode();

                using Stream stream = responseMessage.Content.ReadAsStream();
                IEnumerable<Schedule> result = JsonSerializer.DeserializeAsync<IEnumerable<Schedule>>(stream).Result;
            }
            catch (Exception e)
            {
                throw new Exception($@"Не удалось получить расписание из центра: 
{nameof(_appConfig.RouteNumber)}={_appConfig.RouteNumber}, {nameof(_appConfig.RmisHubUrl)}={_appConfig.RmisHubUrl} 
{Environment.NewLine}Детали: {Environment.NewLine}{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}