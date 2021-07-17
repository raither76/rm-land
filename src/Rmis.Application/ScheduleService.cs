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
            DateTime fromDate = DateTime.Now.Date;

            _logger.LogInformation($"Запуск процедуры сихронизации расписания в соответствии с данными сервиса \"Яндекс.Расписание\", начиная с даты: {fromDate:dd.MM.yyyy}");

            //  Получаем справочник маршрутов Сапсап.
            Dictionary<string, Route> routeByNumber = _context.RouteRepository.GetAll()
                                                        .ToDictionary(k => k.Number.ToString());
                
            //  Список станций.
            Dictionary<string, Station> stationByYaCode = _context.StationRepository.ToDictionary(k => k.YaCode);
            
            foreach (Direction direction in _context.DirectionRepository.GetAll()
                                                                        .ToList())
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
                
                //  Обработка маршрутов.
                Dictionary<string, YandexThread> yaThreadByUid = new Dictionary<string, YandexThread>();

                foreach (KeyValuePair<string, YandexSchedule> yandexSchedulePair in yandexScheduleByKey)
                {
                    Schedule schedule = null;
                    YandexSchedule yaSchedule = yandexSchedulePair.Value; //  Элемент расписания от Яндекса.
                    
                    string yaRouteNumber = yaSchedule.thread.GetNormalizedNumber();
                    
                    #region Инициализация маршрута
                    
                    Route route = null;
                    if (!routeByNumber.ContainsKey(yaRouteNumber))
                    {
                        route = new Route
                        {
                            Direction = direction,
                            Number = int.Parse(yaRouteNumber),
                            YaId = yaSchedule.thread.uid,
                            CreateDate = DateTimeOffset.Now,
                            ModifyDate = DateTimeOffset.Now
                        };

                        routeByNumber[route.Number.ToString()] = route;
                    }
                    else
                        route = routeByNumber[yaRouteNumber];
                    
                    //  Наполнение маршрута остановками.
                    if (!yaThreadByUid.ContainsKey(yaSchedule.thread.uid))
                    {
                        YandexThread thread = _scheduleProvider.GetThread(yaSchedule.thread.uid);
                        yaThreadByUid[yaSchedule.thread.uid] = thread;

                        foreach (YaStop yaStop in thread.stops)
                        {
                            Stop stop = route.Stops.FirstOrDefault(s => s.Station.YaCode == yaStop.station.code);
                            
                            if (stop == null)
                            {
                                stop = new Stop
                                {
                                    CreateDate = DateTimeOffset.Now
                                };
                                
                                route.Stops.Add(stop);
                            }
                            
                            stop.Duration = yaStop.duration;
                            stop.StopTime = yaStop.stop_time;
                            stop.ModifyDate = DateTimeOffset.Now;;
                            
                            Station station = null;
                            if (stationByYaCode.ContainsKey(yaStop.station.code))
                                station = stationByYaCode[yaStop.station.code];
                            else
                            {
                                station = new Station
                                {
                                    YaCode = yaStop.station.code,
                                    CreateDate = DateTimeOffset.Now,
                                };

                                stationByYaCode[station.YaCode] = station;
                            }

                            station.DisplayName = yaStop.station.title;
                            station.ModifyDate = DateTimeOffset.Now;

                            if(stop.Station?.Id == 0 || stop.Station?.Id != station.Id)
                                stop.Station = station;
                        }
                        
                        // Удаление неактуальных остановок.
                        List<Stop> stops = new List<Stop>(route.Stops);
                        stops.ForEach(s =>
                        {
                            if (thread.stops.Any(ss => ss.station.code == s.Station.YaCode))
                                return;
                            
                            route.Stops.Remove(s);
                        });
                    }
                    
                    #endregion
                    
                    if (!scheduleByKey.ContainsKey(yandexSchedulePair.Key))
                    {
                        #region Создание нового элемента

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
                        scheduleByKey[schedule.GetKey()] = schedule;

                        #endregion
                    }
                    else
                    {
                        #region Обновление существующего

                        schedule = scheduleByKey[yandexSchedulePair.Key];
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
            DateTime fromDate = DateTime.Now.Date;

            _logger.LogInformation($"Запуск процедуры сихронизации расписания в соответствии с данными Реестра в Google Sheets, начиная с даты: {fromDate:dd.MM.yyyy}");

            //  Получаем справочник маршрутов Сапсап.
            Dictionary<int, Route> routeByNumber = _context.RouteRepository
                .Include(r => r.Direction)
                .ToDictionary(k => k.Number);

            //  Получение текущих элементов расписания.
            List<Schedule> schedules = _context.ScheduleRepository
                .GetAllByFromDate(fromDate).ToList();

            //  Получение расписаний из Реестра размещенного в Google Sheets.
            Dictionary<string, GoogleSchedule> googleScheduleByKey = _googleScheduleProvider.GetSchedules(fromDate)
                .ToDictionary(k => k.GetKey());

            foreach (KeyValuePair<string, GoogleSchedule> googleSchedulePair in googleScheduleByKey)
            {
                GoogleSchedule googleSchedule = googleSchedulePair.Value; //  Элемент расписания из Реестра.

                string[] trainNumberChunks = googleSchedule.TrainNumber.Split("+", StringSplitOptions.RemoveEmptyEntries);
                foreach (string trainNumber in trainNumberChunks)
                {
                    IEnumerable<Schedule> schedulesByRouteNumber = schedules.Where(s => s.Route.Number == googleSchedule.Number && s.Date == googleSchedule.Date);
                    Schedule schedule = schedulesByRouteNumber.FirstOrDefault(s => s.TrainNumber == trainNumber);
                    if (schedule == null)
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
                            TrainNumber = trainNumber,
                            TrainDriver = googleSchedule.TrainDriver,
                            ArrivalDate = googleSchedule.ArrivalDate,
                            DepartureDate = googleSchedule.DepartureDate,
                            CreateDate = DateTimeOffset.Now,
                            ModifyDate = DateTimeOffset.Now
                        };

                        schedules.Add(schedule);
                        _context.ScheduleRepository.Add(schedule);

                        #endregion
                    }
                    else
                    {
                        #region Обновление существующего

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

                        if (schedule.TrainNumber != googleSchedule.TrainNumber)
                        {
                            _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" был актуализирован номер поезда. Старое значение: {schedule.TrainNumber}. Новое значение: {googleSchedule.TrainNumber}");
                            schedule.TrainNumber = googleSchedule.TrainNumber;
                        }

                        if (schedule.TrainDriver != googleSchedule.TrainDriver)
                        {
                            _logger.LogInformation($"По маршруту номер \"{schedule.Route.Number}\" был актуализирован машинист поезда. Старое значение: {schedule.TrainDriver}. Новое значение: {googleSchedule.TrainDriver}");
                            schedule.TrainDriver = googleSchedule.TrainDriver;
                        }

                        #endregion
                    }

                    //  Устанавливаем признак, для того чтобы данные не перезатерлись данными из Яндекс.
                    schedule.IsSynchronized = true;
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

        public IEnumerable<ScheduleVm> GetSchedulesByRouteNumber(string trainNumber)
        {
            if (string.IsNullOrEmpty(trainNumber))
                throw new ArgumentNullException(nameof(trainNumber));

            return _context.ScheduleRepository.GetActualAllByTrainNumber(trainNumber).Select(s => ScheduleVm.CreateFrom(s));
        }
    }
}