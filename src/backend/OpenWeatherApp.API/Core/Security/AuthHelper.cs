using OpenWeatherApp.API.Controllers;

namespace OpenWeatherApp.API.Core.Security
{
    public class AuthHelper : IAuthHelper
    {
        private readonly IConfiguration _configuration;

        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValidApiKey(string requestApiKey)
        {
            if (string.IsNullOrWhiteSpace(requestApiKey))
                return false;

            if (string.Equals(requestApiKey, _configuration.GetValue<string>(Constants.AppKey1)) ||
                string.Equals(requestApiKey, _configuration.GetValue<string>(Constants.AppKey2)) ||
                string.Equals(requestApiKey, _configuration.GetValue<string>(Constants.AppKey3)) ||
                string.Equals(requestApiKey, _configuration.GetValue<string>(Constants.AppKey4)) ||
                string.Equals(requestApiKey, _configuration.GetValue<string>(Constants.AppKey5)))
                return true;

            return false;
        }
    }
}
