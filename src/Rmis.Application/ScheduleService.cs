using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rmis.Application.Abstract;
using Rmis.Domain;
using Rmis.Persistence.Abstract;
using Rmis.Yandex.Schedule;
using Rmis.Yandex.Schedule.Abstract;

namespace Rmis.Application
{
    internal class ScheduleService : IScheduleService
    {
        private readonly IYandexScheduleProvider _scheduleProvider;
        private readonly IRmisDbContext _context;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IYandexScheduleProvider scheduleProvider, IRmisDbContext context, ILogger<ScheduleService> logger)
        {
            _scheduleProvider = scheduleProvider;
            _context = context;
            _logger = logger;
        }

        public void SyncSchedulesFromYandex()
        {
            // string fromStationCode = "s9602494";
            // string toStationCode = "s2006004";
            DateTime fromDate = DateTime.Now.Date;
            
            _logger.LogInformation($"Запуск процедуры сихронизации расписания в соответствии с данными сервиса \"Яндекс.Расписание\", начиная с даты: {fromDate:dd.MM.yyyy}");

            //  Получаем справочник маршрутов Сапсап.
            Dictionary<string, Route> routeByNumber = _context.RouteRepository
                .Include(r => r.Direction)
                .ToDictionary(k => k.Number.ToString());
            
            foreach (Direction direction in _context.DirectionRepository.GetAll().ToList())
            {
                _logger.LogInformation($"Актуализация расписания по направлению \"{direction.DisplayName}\" ...");
                
                //  Получение текущих элементов расписания.
                Dictionary<string, Schedule> scheduleByKey = _context.ScheduleRepository
                    .GetAllByDirectionAndFromDate(direction.Id, fromDate)
                    .ToDictionary(k => k.GetKey());
                
                //  Получение расписаний от сервиса Яндекс.
                Dictionary<string, YandexSchedule> yandexScheduleByKey = _scheduleProvider
                    .GetSchedules(direction.FromStation.YaCode, direction.ToStation.YaCode, fromDate)
                    .ToDictionary(k => k.GetKey());

                foreach (KeyValuePair<string, YandexSchedule> yandexSchedulePair in yandexScheduleByKey)
                {
                    Schedule schedule = null;
                    YandexSchedule yaSchedule = yandexSchedulePair.Value;   //  Элемент расписания от Яндекса.
                    string yaRouteNumber = yaSchedule.thread.GetNormalizedNumber();
                    if (!scheduleByKey.ContainsKey(yandexSchedulePair.Key))
                    {
                        #region Создание нового элемента
                        
                        Route route = null;
                        if (!routeByNumber.ContainsKey(yaRouteNumber))
                        {
                            route = new Route
                            {
                                Direction = direction,
                                Number = int.Parse(yaRouteNumber),
                                TrainNumber = int.Parse(yaRouteNumber),
                                CreateDate = DateTimeOffset.Now,
                                ModifyDate = DateTimeOffset.Now
                            };

                            routeByNumber[route.Number.ToString()] = route;
                        }
                        else 
                            route = routeByNumber[yaRouteNumber]; 
                        
                        schedule = new Schedule()
                        {
                            Date = DateTime.Parse(yaSchedule.start_date),
                            Route = route,
                            ArrivalDate = yaSchedule.arrival,
                            DepartureDate = yaSchedule.departure,
                            CreateDate = DateTimeOffset.Now,
                            ModifyDate = DateTimeOffset.Now
                        };
                        
                        _context.ScheduleRepository.Add(schedule);
                        
                        #endregion
                    }
                    else
                    {
                        #region Обновление существующего
                        
                        schedule = scheduleByKey[yandexSchedulePair.Key];
                        if (!schedule.IsSynchronized)   //  Обновляем расписание в том случае, если оно не синхронизировано с реестром. Тем самым считаем информацию из реестра более приоритетной.
                        {
                            if (schedule.ArrivalDate != yaSchedule.arrival)
                            {
                                _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" была актуализирована дата прибытия. Старое значение: {schedule.ArrivalDate:dd.MM.yyyy hh:mm}. Новое значение: {yaSchedule.arrival:dd.MM.yyyy hh:mm}");
                                schedule.ArrivalDate = yaSchedule.arrival;
                            }

                            if (schedule.DepartureDate != yandexSchedulePair.Value.departure)
                            {
                                _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" была актуализирована дата посадки. Старое значение: {schedule.DepartureDate:dd.MM.yyyy hh:mm}. Новое значение: {yaSchedule.departure:dd.MM.yyyy hh:mm}");
                                schedule.DepartureDate = yandexSchedulePair.Value.departure;
                            }
                        }
                        
                        #endregion
                    }
                }
            }

            _logger.LogInformation("Сохранение изменений ...");
            _context.SaveChanges();
            _logger.LogInformation("Изменения сохранены");
            
            #region Удаление неактуальных Элементов расписания
            
            // //  Removing not actual entities.
            // _logger.LogInformation("Удаление неактуальных расписаний ...");
            // int deletedCount = _context.ScheduleRepository.RemoveBeforeDate(fromDate);
            // _logger.LogInformation("Количество удаленных неактуальных расписаний: {DeletedCount}", deletedCount);
            
            #endregion
            
            _logger.LogInformation("Расписание синхронизировано");
        }
    }
}