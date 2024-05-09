using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.Model;

namespace OpenWeatherApp.API.Service
{
    public class OpenWeatherMapService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly IGeocodingService _geocodingService;

        public OpenWeatherMapService(HttpClient httpClient, IGeocodingService geocodingService, IOptions<AppSettings> options)
        {
            _httpClient = httpClient;
            _appSettings = options.Value;
            _geocodingService = geocodingService;
        }

        public async Task<OpenWeatherMapApiResponse> GetWeather(string location)
        {
            var geoLocation = await _geocodingService.GetLocation(location);
            string apiKey = _appSettings.OpenWeatherMapApiKey;
            string apiUrl = $"/data/2.5/weather?lat={geoLocation.Latitude}&lon={geoLocation.Longitude}&appid={apiKey}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OpenWeatherMapApiResponse>(jsonResponse);
            }

            throw new Exception("An error occured when fetching the weather data");
        }
    }
}
