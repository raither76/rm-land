﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rmis.Google.Sheets.Abstract;

namespace Rmis.Google.Sheets
{
    public class GoogleSheetsScheduleProvider : IGoogleSheetsScheduleProvider
    {
        private static string[] Scopes = {SheetsService.Scope.SpreadsheetsReadonly};
        private const string _dataRange = "Velaro!A2:K"; 

        private readonly ILogger<GoogleSheetsScheduleProvider> _logger;
        private readonly GoogleSheetsConfig _config;

        public GoogleSheetsScheduleProvider(ILogger<GoogleSheetsScheduleProvider> logger, GoogleSheetsConfig options)
        {
            _logger = logger;
            _config = options;
        }

        /// <summary>
        /// Метод получения расписания из реестра, размещенного в сервисе Google Sheets
        /// Детали реализации: https://medium.com/@semuserable/net-core-google-sheets-api-read-write-5edd919868e3
        /// </summary>
        public IEnumerable<GoogleSchedule> GetSchedules(DateTime fromDate)
        {
            try
            {
                IEnumerable<GoogleSchedule> result = new List<GoogleSchedule>();
                SpreadsheetsResource.ValuesResource serviceValues = this.GetSheetsService().Spreadsheets.Values;

                SpreadsheetsResource.ValuesResource.GetRequest request = serviceValues.Get(_config.SheetId, _dataRange);

                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null)
                {
                    result = values.Where(r => r[0] != null && !string.IsNullOrEmpty(r[0].ToString()) && r[3].ToString()?.Trim().Length == 3 && this.CreateDateFromString(r[0].ToString()) >= fromDate)
                        .Select(r => this.CreateModelFromRow(r)).Where(s => s != null);
                }
                else
                    _logger.LogInformation("No data found.");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка при получении расписания расположенного в Google Sheets");
                throw;
            }
        }

        private SheetsService GetSheetsService()
        {
            if (!File.Exists(_config.CredentialFileName))
                throw new FileNotFoundException(_config.CredentialFileName);
            
            using (var stream = new FileStream(_config.CredentialFileName, FileMode.Open, FileAccess.Read))
            {
                BaseClientService.Initializer serviceInitializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(Scopes)
                };
                
                return new SheetsService(serviceInitializer);
            }
        }

        private GoogleSchedule CreateModelFromRow(IList<object> row)
        {
            try
            {
                if (row == null)
                    throw new ArgumentNullException(nameof(row));

                string dateString = row[0]?.ToString();
                DateTime date = this.CreateDateFromString(dateString);

                string routeNumberString = row[3]?.ToString();
                if (!int.TryParse(routeNumberString, out var routeNumber))
                    throw new InvalidCastException($"Не удалость преобразовать значение \"{routeNumberString}\" к целочисленному типу");

                string departureTime = row[5]?.ToString();
                string arrivalTime = row[6]?.ToString();

                string trainNumber = row[1].ToString();
                if (string.IsNullOrEmpty(trainNumber))
                    throw new Exception($"В оборотной ведомости не указан номер поезда по маршруту номер {routeNumberString} за дату {dateString}");

                string trainDriver = row.Count >= 11 ? row[10].ToString() : null;
                
                GoogleSchedule result = new GoogleSchedule
                {
                    Date = date,
                    Number = routeNumber,
                    TrainNumber = trainNumber,
                    DepartureDate = this.ParseDateFromTime(date, departureTime),
                    ArrivalDate = this.ParseDateFromTime(date, arrivalTime),
                    TrainDriver = trainDriver
                };

                if (result.DepartureDate > result.ArrivalDate)  //  Если время прибытия выпадает на след день, то корректируем некорректно посчитанную дату, прибавляя 1 день.
                    result.ArrivalDate = result.ArrivalDate.AddDays(1);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, e.Message);
                return null;
            }
        }

        private DateTimeOffset ParseDateFromTime(DateTime initialDate, string timeString)
        {
            if (initialDate == DateTime.MinValue)
                throw new ArgumentNullException(nameof(initialDate));

            string[] timeChunks = timeString.Split(":", StringSplitOptions.RemoveEmptyEntries);
            if(timeChunks.Length == 0 || timeChunks.Length > 2)
                throw new InvalidCastException($"Не удалость разбить строку на отдельные части с помощью разделителя \":\": {timeString}");
            
            if(!int.TryParse(timeChunks[0], out var hours))
                throw new InvalidCastException($"Не далось преобразоваь значение \"{timeChunks[0]}\" к целочисленному типу. Исходное значение: {timeString}");
            
            if(!int.TryParse(timeChunks[1], out var minutes))
                throw new InvalidCastException($"Не далось преобразоваь значение \"{timeChunks[1]}\" к целочисленному типу. Исходное значение: {timeString}");

            DateTimeOffset result = new DateTimeOffset(initialDate.Year, initialDate.Month, initialDate.Day, hours, minutes, 0, TimeSpan.FromHours(3));
            return result;
        }

        private DateTime CreateDateFromString(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                throw new ArgumentNullException(nameof(dateString));
            
            string[] dateStringChunks = dateString.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (dateStringChunks.Length == 0 || dateStringChunks.Length > 3)
                throw new InvalidCastException($"Не удалость разбить строку на отдельные части с помощью разделителя \".\": {dateString}");

            if (!int.TryParse(dateStringChunks[2], out var year))
                throw new InvalidCastException($"Не далось преобразоваь значение \"{dateStringChunks[2]}\" к целочисленному типу. Исходное значение: {dateString}");
            
            if (!int.TryParse(dateStringChunks[1], out var month))
                throw new InvalidCastException($"Не далось преобразоваь значение \"{dateStringChunks[1]}\" к целочисленному типу. Исходное значение: {dateString}");
            
            if (!int.TryParse(dateStringChunks[0], out var day))
                throw new InvalidCastException($"Не далось преобразоваь значение \"{dateStringChunks[0]}\" к целочисленному типу. Исходное значение: {dateString}");

            DateTime result = new DateTime(year, month, day);

            return result;
        }
    }
}