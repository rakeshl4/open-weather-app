using Microsoft.AspNetCore.Mvc;
using OpenWeatherApp.API.Model;
using OpenWeatherApp.API.Service;

namespace OpenWeatherApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RateLimitConfig(MaxRequestsAllowed = 5, TimeWindowInHours = 1)]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        /// <summary>
        /// Get the weather information for the specified location.
        /// </summary>
        /// <param name="location">The location in the format 'city, country'.</param>
        /// <returns>The weather description for the specified location.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "q")] string? location)
        {
            var country = string.Empty;
            var city = string.Empty;

            if (string.IsNullOrEmpty(location))
                return BadRequest("Location is required");

            if (!string.IsNullOrEmpty(location))
            {
                var parts = location.Split(',');
                if (parts.Length == 2)
                {
                    city = parts[0].Trim();
                    country = parts[1].Trim();
                }
            }

            if (string.IsNullOrEmpty(country))
                return BadRequest("Country is required.");

            if (string.IsNullOrEmpty(city))
                return BadRequest("City is required.");

            try
            {
                var weatherInfo = await _weatherService.GetWeather(location);
                return Ok(weatherInfo.Weather.FirstOrDefault()?.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when getting weather information.");
                return StatusCode(500);
            }
        }
    }
}
