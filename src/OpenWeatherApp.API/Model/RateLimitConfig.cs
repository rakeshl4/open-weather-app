namespace OpenWeatherApp.API.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RateLimitConfig : Attribute
    {
        public int TimeWindowInHours { get; set; }
        public int MaxRequestsAllowed { get; set; }
    }
}
