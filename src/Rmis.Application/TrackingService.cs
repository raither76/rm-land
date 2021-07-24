using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Rmis.Application.Abstract;
using Rmis.Domain;
using Rmis.Persistence.Abstract;

namespace Rmis.Application
{
    internal class TrackingService : ITrackingService
    {
        private readonly ILogger<TrackingService> _logger;
        private readonly IRmisDbContext _context;

        public TrackingService(ILogger<TrackingService> logger, IRmisDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public void SaveTrackInfo(TrackInfoDto trackInfoDto)
        {
            try
            {
                _logger.LogInformation("Запуск обработки текущего местоположения поезда");
                
                if (trackInfoDto == null)
                    throw new ArgumentNullException(nameof(trackInfoDto));
                
                _context.TrackInfoRepository.Add(new()
                {
                    Date = DateTime.Now,
                    Latitude = trackInfoDto.Latitude,
                    Longitude = trackInfoDto.Longitude,
                    Speed = trackInfoDto.Speed,
                    TrainNumber = trackInfoDto.TrainNumber,
                    CreateDate = DateTimeOffset.Now,
                    ModifyDate = DateTimeOffset.Now
                });

                _context.SaveChanges();
                
                _logger.LogInformation($"Текущее местоположение поезда(номер: {trackInfoDto.TrainNumber}) обработано");
            }
            catch (Exception e)
            {
                string message = $"Ошибка при обработке текущего местоположения поезда номер: {trackInfoDto?.TrainNumber}";
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
        }
        
        public TrackInfoDto GetLastTrackInfo(string trainNumber)
        {
            try
            {
                _logger.LogInformation($"Запуск процедуры получения текущего местоположения поезда номер: {trainNumber}");
                
                if (string.IsNullOrEmpty(trainNumber))
                    throw new ArgumentNullException(nameof(trainNumber));

                TrackInfo trackInfo = _context.TrackInfoRepository.Where(t => t.TrainNumber == trainNumber)
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefault();

                if (trackInfo == null)
                {
                    _logger.LogWarning($"Отсутствуют данные от текущем местоположении поезда номер: {trainNumber}");
                    return null;
                }

                TrackInfoDto result = new()
                {
                    Latitude = trackInfo.Latitude,
                    Longitude = trackInfo.Longitude,
                    Speed = trackInfo.Speed,
                    TrainNumber = trackInfo.TrainNumber
                };

                _logger.LogInformation($"Получены данные о текущем местоположении поезда номер: {trainNumber}");
                
                return result;
            }
            catch (Exception e)
            {
                string message = $"Ошибка при получении информации о текущеем местоположении поезда номер: {trainNumber}";
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
        }
    }
}