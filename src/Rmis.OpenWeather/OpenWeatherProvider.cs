using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Rmis.OpenWeather
{
    public class OpenWeatherProvider : IOpenWeatherProvider
    {
        private readonly OpenWeatherConfig _config;
        private readonly ILogger<OpenWeatherProvider> _logger;

        public OpenWeatherProvider(ILogger<OpenWeatherProvider> logger, IOptions<OpenWeatherConfig> options)
        {
            _logger = logger;
            _config = options.Value;
        }

        public WeatherResponse GetWeatherInfoByGeo(double latitude, double longitude)
        {
            try
            {
                if (latitude == 0)
                    throw new ArgumentNullException(nameof(latitude));
                
                if (longitude == 0)
                    throw new ArgumentNullException(nameof(longitude));
                
                using HttpClient client = new HttpClient();
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "appid", _config.ApiKey },
                    { "units", "metric" },
                    { "lang", "ru" },
                    { "lon", longitude.ToString() },
                    { "lat", latitude.ToString() }
                };
                
                string url = QueryHelpers.AddQueryString(_config.Uri, parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                
                HttpResponseMessage responseMessage = client.Send(requestMessage);

                responseMessage.EnsureSuccessStatusCode();
                
                WeatherResponse result = this.GetMessageData<WeatherResponse>(responseMessage);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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