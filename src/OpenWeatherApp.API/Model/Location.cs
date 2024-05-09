using Newtonsoft.Json;

namespace OpenWeatherApp.API.Model
{
    public class Location
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }
}
