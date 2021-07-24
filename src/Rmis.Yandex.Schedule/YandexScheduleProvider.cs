using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public IEnumerable<YandexSchedule> GetSchedules(string fromYaStationCode, string toYaStationCode, DateTime fromDate)
        {
            try
            {
                if (fromDate.Date == DateTime.MinValue)
                    throw new ArgumentException(nameof(fromDate));

                List<YandexSchedule> schedules = new List<YandexSchedule>();
                DateTime from = fromDate;
                DateTime to = fromDate.AddDays(_config.ScheduledDaysCount);
                int limit = _config.Limit;
                
                while(from <= to)
                {
                    Console.WriteLine(from.ToString("dd.MM.yy"));
                    int offset = 0;

                    YandexApiResult<YandexSchedule> data;
                    do
                    {
                        data = this.GetScheduleByDate(fromYaStationCode, toYaStationCode, from.ToString("yyyy-MM-dd"), offset, limit);
                        if(data.segments.Count > 0)
                            schedules.AddRange(data.segments);
                        
                        offset += limit;
                    } while (data.pagination.total >= offset);
                    
                    from = from.AddDays(1);
                }
                
                IEnumerable<YandexSchedule> result = schedules.Where(t => t.thread.transport_type == "train" && Regex.IsMatch(t.thread.number, _config.RouteNumberFilteringRegExp)).ToList();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Ошибка при получении расписания от сервиса Яндекс.Расписание");
                throw;
            }
        }

        public YandexThread GetThread(string uid, string yaDate)
        {
            try
            {
                if (string.IsNullOrEmpty(uid))
                    throw new ArgumentNullException(nameof(uid));
            
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "uid", uid },
                    { "date", yaDate }
                };

                YandexThread result = this.GetDataFromYandex<YandexThread>(_config.ThreadUri, parameters);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($@"Не удалось получить данные о нитке от сервиса Ярдекс.Расписание по следущим параметрам: 
{nameof(uid)}={uid}{Environment.NewLine}Детали: {Environment.NewLine}{Environment.NewLine}{e.Message}", e);
            }
        }

        private YandexApiResult<YandexSchedule> GetScheduleByDate(string fromYaStationCode, string toYaStationCode, string date, int offset, int limit)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "lang", "ru_RU"},
                    { "from", fromYaStationCode },
                    { "to", toYaStationCode },
                    { "date", date },
                    { "transport_types", "train" },
                    { "offset", offset.ToString() },
                    { "limit", limit.ToString() }
                };
                
                YandexApiResult<YandexSchedule> result = this.GetDataFromYandex<YandexApiResult<YandexSchedule>>(_config.ScheduleUri, parameters);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($@"Не удалось получить расписание от сервиса Ярдекс.Расписание по следущим параметрам: 
{nameof(fromYaStationCode)}={fromYaStationCode}, {nameof(toYaStationCode)}={toYaStationCode}, {nameof(date)}={date}, {nameof(limit)}={limit}, {nameof(offset)}={offset} 
{Environment.NewLine}Детали: {Environment.NewLine}{Environment.NewLine}{e.Message}", e);
            }
        }

        private T GetMessageData<T>(HttpResponseMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            
            using Stream stream = message.Content.ReadAsStream();
            T data = JsonSerializer.DeserializeAsync<T>(stream).Result;
            
            return data;
        }

        private TResponse GetDataFromYandex<TResponse>(string url, Dictionary<string, string> parameters = null)
        {
            using HttpClient client = new HttpClient();

            if(parameters != null)
                url = QueryHelpers.AddQueryString(url, parameters);
            
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Authorization", _config.ApiKey);

            HttpResponseMessage responseMessage = client.Send(requestMessage);
            if (responseMessage.StatusCode == HttpStatusCode.BadRequest || responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                YandexErrorResponse errorResponse = this.GetMessageData<YandexErrorResponse>(responseMessage);
                throw new YandexException(errorResponse);
            }

            responseMessage.EnsureSuccessStatusCode();
                
            TResponse result = this.GetMessageData<TResponse>(responseMessage);
            return result;
        }

        public IEnumerable<YandexStation> GetAllStations()
        {
            try
            {
                YandexAllStationsResponse response = this.GetDataFromYandex<YandexAllStationsResponse>(_config.AllStationsUri);

                //  Смотрим только по России.
                string russianYaCoutryCode = "l225";
                string transportType = "train";
                return response.countries.Where(c => c.codes.yandex_code == russianYaCoutryCode)
                        .SelectMany(c => c.regions.SelectMany(r => r.settlements.SelectMany(s => s.stations))).Where(s => s.transport_type == transportType);
            }
            catch (Exception e)
            {
                throw new Exception($@"Ошибка при получении списка всех станций: 
{nameof(_config.AllStationsUri)}={_config.AllStationsUri} 
{Environment.NewLine}Детали: {Environment.NewLine}{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}