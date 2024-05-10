namespace OpenWeatherApp.API.Core.Security
{
    public interface IAuthHelper
    {
        bool IsValidApiKey(string userApiKey);
    }
}
