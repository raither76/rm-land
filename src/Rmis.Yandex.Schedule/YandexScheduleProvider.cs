﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

        public List<YandexSchedule> GetSchedules(string fromYaStationCode, string toYaStationCode, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            try
            {
                if (fromDate.Date > toDate.Date)
                    throw new ArgumentException($"fromDate({fromDate:dd.MM.yyyy}) mustn't be greater then toDate({toDate:dd.MM.yyyy})");

                List<YandexSchedule> result = new List<YandexSchedule>();
                DateTimeOffset from = fromDate;
                int limit = _config.Limit;
                
                while(from < toDate)
                {
                    Console.WriteLine(from.ToString("dd.MM.yy"));
                    int offset = 0;

                    YandexApiResult<YandexSchedule> data;
                    do
                    {
                        data = this.GetScheduleByDate(fromYaStationCode, toYaStationCode, from.ToString("yyyy-MM-dd"), offset, limit);
                        if(data.segments.Count > 0)
                            result.AddRange(data.segments);
                        
                        offset += limit;
                    } while (data.pagination.total >= offset);
                    
                    from = from.AddDays(1);
                }
                
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Ошибка при получении расписания от сервиса Яндекс.Расписание");
                throw;
            }
        }

        private YandexApiResult<YandexSchedule> GetScheduleByDate(string fromYaStationCode, string toYaStationCode, string date, int offset, int limit)
        {
            try
            {
                using HttpClient client = new HttpClient();
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "lang", "ru_RU"},
                    { "from", fromYaStationCode },
                   // { "to", toYaStationCode },
                    { "date", date },
                    { "transport_types", "train" },
                    { "offset", offset.ToString() },
                    { "limit", limit.ToString() }
                };

                string url = QueryHelpers.AddQueryString(_config.ScheduleUri, parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("Authorization", _config.ApiKey);

                HttpResponseMessage responseMessage = client.Send(requestMessage);
                if (responseMessage.StatusCode == HttpStatusCode.BadRequest || responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    YandexErrorResponse errorResponse = this.GetMessageData<YandexErrorResponse>(responseMessage);
                    throw new YandexException(errorResponse);
                }

                responseMessage.EnsureSuccessStatusCode();
                
                YandexApiResult<YandexSchedule> result = this.GetMessageData<YandexApiResult<YandexSchedule>>(responseMessage);
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
    }
}