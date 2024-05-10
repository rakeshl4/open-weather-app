namespace OpenWeatherApp.API.Model
{
    public class ClientRequestStatistics
    {
        public DateTime LastResponseTime { get; set; }
        public int NumberOfRequests { get; set; }
        public DateTime CacheValueExpiresAt { get; set; }
    }
}
