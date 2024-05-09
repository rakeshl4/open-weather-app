using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OpenWeatherApp.API.Controllers;
using OpenWeatherApp.API.Service;
using OpenWeatherApp.Model;

namespace OpenWeatherApp.Test.Controllers
{
    [TestClass]
    public class WeatherControllerTest
    {
        private readonly Mock<ILogger<WeatherController>> _loggerMock;
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly WeatherController _weatherController;

        public WeatherControllerTest()
        {
            _loggerMock = new Mock<ILogger<WeatherController>>();
            _weatherServiceMock = new Mock<IWeatherService>();
            _weatherController = new WeatherController(_loggerMock.Object, _weatherServiceMock.Object);
        }

        [TestMethod]
        public async Task Get_WithValidLocation_ReturnsWeatherDescription()
        {
            string location = "London, UK";
            var weatherInfo = new OpenWeatherMapApiResponse
            {
                Weather = new List<Weather>()
                {   new Weather
                    {
                        Description = location
                    }
               }
            };

            _weatherServiceMock.Setup(x => x.GetWeather(location)).ReturnsAsync(weatherInfo);
            var result = await _weatherController.Get(location) as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(weatherInfo.Weather.FirstOrDefault()?.Description, result.Value);
        }

        [TestMethod]
        public async Task Get_WithEmptyLocation_ReturnsBadRequest()
        {
            string location = "";
            var result = await _weatherController.Get(location) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Location is required", result.Value);
        }

        [TestMethod]
        public async Task Get_WithEmptyCity_ReturnsBadRequest()
        {
            string location = ",UK";
            var result = await _weatherController.Get(location) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("City is required.", result.Value);
        }

        [TestMethod]
        public async Task Get_WithEmptyCountry_ReturnsBadRequest()
        {
            string location = "London,";
            var result = await _weatherController.Get(location) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Country is required.", result.Value);
        }

        [TestMethod]
        public async Task Get_WithException_ReturnsStatusCode500()
        {
            string location = "London, UK";
            _weatherServiceMock.Setup(x => x.GetWeather(location)).ThrowsAsync(new Exception());
            var result = await _weatherController.Get(location) as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}