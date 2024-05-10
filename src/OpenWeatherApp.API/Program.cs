using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Core;
using OpenWeatherApp.API.Core.Security;
using OpenWeatherApp.API.Service;

namespace OpenWeatherApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddOptions();
            builder.Services.Configure<AppSettings>(builder.Configuration);
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IAuthHelper, AuthHelper>();
            builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(builder.Configuration[Constants.OpenWeatherMapApiBaseUrl].ToString());
            });
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherMapService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(builder.Configuration[Constants.OpenWeatherMapApiBaseUrl].ToString());
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(builder => builder
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
