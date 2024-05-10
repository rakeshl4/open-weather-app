using OpenWeatherApp.API.Controllers;

namespace OpenWeatherApp.API.Core.Security
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthHelper _authHelper;

        public ApiKeyMiddleware(RequestDelegate next, IAuthHelper authHelper)
        {
            _next = next;
            _authHelper = authHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Query.ContainsKey(Constants.AppId))
            {
                context.Response.StatusCode = 400; //Bad Request                
                await context.Response.WriteAsync("The API key is missing");
                return;
            }
            else
            {
                var appId = context.Request.Query[Constants.AppId].ToString();
                if (!_authHelper.IsValidApiKey(appId))
                {
                    context.Response.StatusCode = 401; //UnAuthorized
                    await context.Response.WriteAsync("Invalid API Key");
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
