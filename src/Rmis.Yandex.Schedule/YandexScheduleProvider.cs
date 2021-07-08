using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rmis.Yandex.Schedule.Abstract;

namespace Rmis.Yandex.Schedule
{
    internal class YandexScheduleProvider : IYandexScheduleProvider 
    {
        private readonly YandexScheduleOptions _config;
        private readonly ILogger<YandexScheduleProvider> _logger;
        

        public YandexScheduleProvider(ILogger<YandexScheduleProvider> logger, IOptions<YandexScheduleOptions> options)
        {
            _logger = logger;
            _config = options.Value;
        }

        public List<YandexSchedule> GetSchedules(string fromYaStationCode, string toYaStationCode, DateTimeOffset fromDate, DateTimeOffset toData)
        {
            try
            {
                using HttpClient client = new HttpClient();
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "lang", "ru_RU"},
                    { "from", fromYaStationCode },
                    { "to", toYaStationCode },
                    { "date", "2021-07-09" },
                    { "transport_types", "train" },
                    { "limit", 1000.ToString() }
                };

                string url = QueryHelpers.AddQueryString(_config.ScheduleUri, parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("Authorization", _config.ApiKey);

                HttpResponseMessage responseMessage = client.Send(requestMessage);
                using Stream stream = responseMessage.Content.ReadAsStream();
                YandexApiResult<YandexSchedule> result = JsonSerializer.DeserializeAsync<YandexApiResult<YandexSchedule>>(stream).Result;

                return result.segments;
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Ошибка при получении расписания от сервиса Яндекс.Расписание");
                throw;
            }
            
            return null;
        }
    }
}