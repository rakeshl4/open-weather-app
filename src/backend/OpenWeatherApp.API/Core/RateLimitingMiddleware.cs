using Microsoft.Extensions.Caching.Memory;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Model;
using System.Net;

namespace OpenWeatherApp.API.Core
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache; // Add the cache dependency

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var decorator = endpoint?.Metadata.GetMetadata<RateLimitConfig>();

            if (decorator is null)
            {
                await _next(context);
                return;
            }

            var key = GetApiKey(context);
            if (string.IsNullOrEmpty(key))
            {
                //the rate limit will not be applied if the api key is missing
                await _next(context);
            }

            var clientStatistics = await GetClientStatisticsByKey(key);
            if (clientStatistics != null &&
                   DateTime.UtcNow < clientStatistics.LastResponseTime.AddHours(decorator.TimeWindowInHours) &&
                   clientStatistics.NumberOfRequests == decorator.MaxRequestsAllowed)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            var numberOfRequestsCompletedSuccessfully = 1;
            if (clientStatistics != null)
                numberOfRequestsCompletedSuccessfully = clientStatistics.NumberOfRequests + 1;
            await SaveClientStatisticsStorage(key, TimeSpan.FromHours(decorator.TimeWindowInHours), numberOfRequestsCompletedSuccessfully);
            await _next(context);
        }

        private async Task<ClientRequestStatistics> GetClientStatisticsByKey(string key)
        {
            return _cache.Get<ClientRequestStatistics>(key);
        }


        private async Task SaveClientStatisticsStorage(string key, TimeSpan absoluteExpiration, int numberOfSuccessfulRequests)
        {
            if (!_cache.TryGetValue(key, out ClientRequestStatistics cacheValue))
            {
                cacheValue = new ClientRequestStatistics()
                {
                    CacheValueExpiresAt = DateTime.UtcNow.Add(absoluteExpiration),
                    LastResponseTime = DateTime.UtcNow,
                    NumberOfRequests = numberOfSuccessfulRequests
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(absoluteExpiration);
                _cache.Set(key, cacheValue, cacheEntryOptions);
            }
            else
            {
                cacheValue.NumberOfRequests = numberOfSuccessfulRequests;
                _cache.Set(key, cacheValue, cacheValue.CacheValueExpiresAt);
            }
        }

        private string GetApiKey(HttpContext context)
        {
            var queryParameters = context.Request.Query;
            if (queryParameters.ContainsKey(Constants.AppId))
                return context.Request.Query[Constants.AppId].ToString();
            return null;
        }
    }
}
