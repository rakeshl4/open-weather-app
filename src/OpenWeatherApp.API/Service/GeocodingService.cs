using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Model;

namespace OpenWeatherApp.API.Service
{
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public GeocodingService(HttpClient httpClient, IOptions<AppSettings> options)
        {
            _httpClient = httpClient;
            _appSettings = options.Value;
        }

        public async Task<Location> GetLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException("Location cannot be null or empty.");

            var country = string.Empty;
            var city = string.Empty;

            var parts = location.Split(',');
            if (parts.Length == 2)
            {
                city = parts[0].Trim();
                country = parts[1].Trim();
            }

            string apiKey = _appSettings.OpenWeatherMapApiKey;
            string apiUrl = $"/geo/1.0/direct?q={city}&appid={apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Location>>(jsonResponse);
                return result?.FirstOrDefault();
            }

            throw new Exception("An error occured when fetching the location data");
        }
    }

}
