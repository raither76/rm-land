using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rmis.Client.Application.Abstract;
using Rmis.Client.Domain;
using Rmis.Client.Persistence.Abstract;

namespace Rmis.Client.Application
{
    internal class ScheduleService : IScheduleService
    {
        private readonly IRmisClientDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IRmisClientDbContext dbContext, AppConfig appConfig, ILogger<ScheduleService> logger)
        {
            _context = dbContext;
            _appConfig = appConfig;
            _logger = logger;
        }
    
        public IEnumerable<Schedule> GetSchedules()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ScheduleDto> GetSchedulesFromHub()
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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<ScheduleDto> result = JsonSerializer.DeserializeAsync<IEnumerable<ScheduleDto>>(stream, options).Result;
                return result;
            }
            catch (Exception e)
            {
                string message = $@"Не удалось получить актуальное расписание от центрального сервиса расписаний: 
{nameof(_appConfig.RouteNumber)}={_appConfig.RouteNumber}, {nameof(_appConfig.RmisHubUrl)}={_appConfig.RmisHubUrl} 
{Environment.NewLine}Детали: {Environment.NewLine}{Environment.NewLine}{e.Message}";
                
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
        }
        
        public void SyncSchedulesFromHub()
        {
            try
            {
                //  Расписания загруженные с центрального сервера.
                IEnumerable<ScheduleDto> schedules = this.GetSchedulesFromHub();
                
                DateTime fromDate = DateTime.Now.Date;
                //  Расписания загруженные с локального сервера.
                Dictionary<string, Schedule> scheduleByKey = _context.ScheduleRepository
                        .Where(s => s.Date >= fromDate)
                        .ToDictionary(k => k.GetKey());

                foreach (ScheduleDto scheduleDto in schedules)
                {
                    string uniqueScheduleKey = scheduleDto.GetKey();
                    
                    if (!scheduleByKey.ContainsKey(uniqueScheduleKey))
                    {
                        #region Создание нового элемента
                        
                        Schedule schedule = new Schedule()
                        {
                            Date = scheduleDto.Date,
                            RouteNumber = scheduleDto.RouteNumber,
                            TrainNumber = scheduleDto.TrainNumber,
                            ArrivalDate = scheduleDto.ArrivalDate,
                            DepartureDate = scheduleDto.DepartureDate,
                            From = scheduleDto.From,
                            To = scheduleDto.To,
                            CreateDate = DateTimeOffset.Now,
                            ModifyDate = DateTimeOffset.Now
                        };
                        
                        _context.ScheduleRepository.Add(schedule);
                        
                        #endregion
                    }
                    else
                    {
                        #region Обновление существующего
                        
                        Schedule schedule = scheduleByKey[uniqueScheduleKey];
             
                        if (schedule.ArrivalDate != scheduleDto.ArrivalDate)
                        {
                            _logger.LogInformation($"По маршруту номер \"{scheduleDto.RouteNumber}\" была актуализирована дата прибытия. Старое значение: {schedule.ArrivalDate:dd.MM.yyyy hh:mm}. Новое значение: {scheduleDto.ArrivalDate:dd.MM.yyyy hh:mm}");
                            schedule.ArrivalDate = scheduleDto.ArrivalDate;
                        }

                        if (schedule.DepartureDate != scheduleDto.DepartureDate)
                        {
                            _logger.LogInformation($"По маршруту номер \"{scheduleDto.RouteNumber}\" была актуализирована дата посадки. Старое значение: {schedule.DepartureDate:dd.MM.yyyy hh:mm}. Новое значение: {scheduleDto.DepartureDate:dd.MM.yyyy hh:mm}");
                            schedule.DepartureDate = scheduleDto.DepartureDate;
                        }
                        
                        if (schedule.TrainNumber != scheduleDto.TrainNumber)
                        {
                            _logger.LogInformation($"По маршруту номер \"{scheduleDto.RouteNumber}\" была актуализирован номер поезда. Старое значение: {schedule.TrainNumber}. Новое значение: {scheduleDto.TrainNumber}");
                            schedule.TrainNumber = scheduleDto.TrainNumber;
                        }
                        
                        if (schedule.From != scheduleDto.From)
                        {
                            _logger.LogInformation($"По маршруту номер \"{scheduleDto.RouteNumber}\" была актуализирован пункт отправления. Старое значение: {schedule.From}. Новое значение: {scheduleDto.From}");
                            schedule.From = scheduleDto.From;
                        }
                        
                        if (schedule.To != scheduleDto.To)
                        {
                            _logger.LogInformation($"По маршруту номер \"{scheduleDto.RouteNumber}\" была актуализирован пункт прибытия. Старое значение: {schedule.To}. Новое значение: {scheduleDto.To}");
                            schedule.To = scheduleDto.To;
                        }
                        
                        #endregion
                    }
                }
                
                _logger.LogInformation("Сохранение изменений ...");
                _context.SaveChanges();
                _logger.LogInformation("Изменения сохранены");
                
                _logger.LogInformation("Расписание синхронизировано");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка в процессе обновления расписания поезда", e);
            }
        }
    }
}