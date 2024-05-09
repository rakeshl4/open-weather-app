using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Model;
using OpenWeatherApp.API.Service;
using OpenWeatherApp.Model;

namespace OpenWeatherApp.API.Tests.Service
{
    [TestClass]
    public class OpenWeatherMapServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IGeocodingService> _geocodingServiceMock;
        private readonly OpenWeatherMapService _weatherService;
        private readonly AppSettings _appSettings;

        public OpenWeatherMapServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://api.openweathermap.org");
            _geocodingServiceMock = new Mock<IGeocodingService>();
            _appSettings = new AppSettings { OpenWeatherMapApiKey = "api-key" };
            _weatherService = new OpenWeatherMapService(_httpClient, _geocodingServiceMock.Object, Options.Create(_appSettings));
        }

        [TestMethod]
        public async Task GetWeather_ReturnsWeatherData_WhenResponseIsSuccessful()
        {
            var location = "London, UK";
            var geoLocation = new Location { Latitude = 40.7128, Longitude = -74.0060 };
            var weatherInfo = new OpenWeatherMapApiResponse
            {
                Weather = new List<Weather>()
                {   new Weather
                    {
                        Description = location
                    }
               }
            };

            _geocodingServiceMock.Setup(x => x.GetLocation(location)).ReturnsAsync(geoLocation);
            var jsonResponse = JsonConvert.SerializeObject(weatherInfo);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            _httpMessageHandlerMock.Protected()
              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
             .ReturnsAsync(responseMessage);

            var result = await _weatherService.GetWeather(location);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetWeather_ThrowsException_WhenResponseIsNotSuccessful()
        {
            try
            {
                var location = "London, UK";
                var geoLocation = new Location { Latitude = 40.7128, Longitude = -74.0060 };
                _geocodingServiceMock.Setup(x => x.GetLocation(location)).ReturnsAsync(geoLocation);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                _httpMessageHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(responseMessage);
                await _weatherService.GetWeather(location);
                Assert.Fail(); // raises AssertionException
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "An error occured when fetching the weather data");
            }
        }
    }
}
