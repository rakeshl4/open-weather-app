using OpenWeatherApp.Model;

namespace OpenWeatherApp.API.Service
{
    public interface IWeatherService
    {
        Task<OpenWeatherMapApiResponse> GetWeather(string location);
    }
}
