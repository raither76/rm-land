using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rmis.Application.Abstract;
using Rmis.Domain;
using Rmis.Google.Sheets;
using Rmis.Google.Sheets.Abstract;
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
        private readonly IGoogleSheetsScheduleProvider _googleScheduleProvider;

        public ScheduleService(IYandexScheduleProvider scheduleProvider, IRmisDbContext context, ILogger<ScheduleService> logger, IGoogleSheetsScheduleProvider googleScheduleProvider)
        {
            _scheduleProvider = scheduleProvider;
            _context = context;
            _logger = logger;
            _googleScheduleProvider = googleScheduleProvider;
        }

        public void SyncSchedulesFromYandex()
        {
            // string fromStationCode = "s9602494";
            // string toStationCode = "s2006004";
            DateTime fromDate = DateTime.Now.Date.AddDays(-1);
            
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
                                TrainNumber = yaRouteNumber,
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
                                schedule.DepartureDate = yaSchedule.departure;
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

        public void SyncSchedulesFromGoogle()
        {
            DateTime fromDate = DateTime.Now.Date.AddDays(-1);
            
            _logger.LogInformation($"Запуск процедуры сихронизации расписания в соответствии с данными Реестра в Google Sheets, начиная с даты: {fromDate:dd.MM.yyyy}");

            //  Получаем справочник маршрутов Сапсап.
            Dictionary<int, Route> routeByNumber = _context.RouteRepository
                .Include(r => r.Direction)
                .ToDictionary(k => k.Number);
            
            //  Получение текущих элементов расписания.
            Dictionary<string, Schedule> scheduleByKey = _context.ScheduleRepository
                .GetAllByFromDate(fromDate)
                .ToDictionary(k => k.GetKey());
            
            //  Получение расписаний из Реестра размещенного в Google Sheets.
            Dictionary<string, GoogleSchedule> googleScheduleByKey = _googleScheduleProvider.GetSchedules(fromDate)
                                                                                .ToDictionary(k => k.GetKey());

            foreach (KeyValuePair<string, GoogleSchedule> googleSchedulePair in googleScheduleByKey)
            {
                Schedule schedule = null;
                GoogleSchedule googleSchedule = googleSchedulePair.Value;   //  Элемент расписания из Реестра.
                if (!scheduleByKey.ContainsKey(googleSchedulePair.Key))
                {
                    #region Создание нового элемента

                    if (!routeByNumber.ContainsKey(googleSchedule.Number))
                    {
                        _logger.LogWarning($"В справочнике маршрутов отсутствует маршрут с номером: {googleSchedule.Number}");
                        continue;
                    }
                    
                    Route route = routeByNumber[googleSchedule.Number]; 
                    
                    schedule = new Schedule()
                    {
                        Date = googleSchedule.Date,
                        Route = route,
                        ArrivalDate = googleSchedule.ArrivalDate,
                        DepartureDate = googleSchedule.DepartureDate,
                        CreateDate = DateTimeOffset.Now,
                        ModifyDate = DateTimeOffset.Now
                    };
                    
                    _context.ScheduleRepository.Add(schedule);
                    
                    #endregion
                }
                else
                {
                    #region Обновление существующего
                    
                    schedule = scheduleByKey[googleSchedulePair.Key];
                    
                    if (schedule.ArrivalDate != googleSchedule.ArrivalDate)
                    {
                        _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" была актуализирована дата прибытия. Старое значение: {schedule.ArrivalDate:dd.MM.yyyy hh:mm}. Новое значение: {googleSchedule.ArrivalDate:dd.MM.yyyy hh:mm}");
                        schedule.ArrivalDate = googleSchedule.ArrivalDate;
                    }

                    if (schedule.DepartureDate != googleSchedule.DepartureDate)
                    {
                        _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" была актуализирована дата посадки. Старое значение: {schedule.DepartureDate:dd.MM.yyyy hh:mm}. Новое значение: {googleSchedule.DepartureDate:dd.MM.yyyy hh:mm}");
                        schedule.DepartureDate = googleSchedule.DepartureDate;
                    }

                    if (schedule.Route.TrainNumber != googleSchedule.TrainNumber)
                    {
                        _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" был актуализирован номер поезда. Старое значение: {schedule.Route.TrainNumber}. Новое значение: {googleSchedule.TrainNumber}");
                        schedule.Route.TrainNumber = googleSchedule.TrainNumber;
                    }
                    
                    #endregion
                }
                
                schedule.IsSynchronized = true;
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