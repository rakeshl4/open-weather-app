using OpenWeatherApp.API.Model;

namespace OpenWeatherApp.API.Service
{
    public interface IGeocodingService
    {
        Task<Location> GetLocation(string location);
    }
}
