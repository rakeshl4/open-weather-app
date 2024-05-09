using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Model;
using OpenWeatherApp.API.Service;
using System.Net;

namespace OpenWeatherApp.Test.Service
{
    [TestClass]
    public class GeocodingServiceTest
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public GeocodingServiceTest()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://api.openweathermap.org");
            _appSettings = new AppSettings { OpenWeatherMapApiKey = "api-key" };
        }

        [TestMethod]
        public async Task GetLocation_ValidLocation_ReturnsLocation()
        {
            var geocodingService = new GeocodingService(_httpClient, Options.Create(_appSettings));
            var locationResponse = new List<Location>
            {
                  new Location {
                       Latitude=  51.5073219 ,
                       Longitude=  -0.1276474
                 }
            };

            var jsonResponse = JsonConvert.SerializeObject(locationResponse);
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var result = await geocodingService.GetLocation("London, UK");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Longitude, -0.1276474);
            Assert.AreEqual(result.Latitude, 51.5073219);
        }

        [TestMethod]
        public async Task GetLocation_InvalidLocation_ThrowsException()
        {
            try
            {
                var geocodingService = new GeocodingService(_httpClient, Options.Create(_appSettings));
                _httpMessageHandlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound
                    });
                await geocodingService.GetLocation("London, UK");
                Assert.Fail(); // raises AssertionException
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "An error occured when fetching the location data");
            }
        }
    }
}
