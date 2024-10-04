using Moq;
using WeatherWise_api.Controllers;
using WeatherWise_api.Services;
using WeatherWise_api.Modal;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace WeatherForecast.tests.Controllers;
public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
    private readonly WeatherForecastController _weatherForecastController;

    public WeatherForecastControllerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _loggerMock = new Mock<ILogger<WeatherForecastController>>();
        _weatherForecastController = new WeatherForecastController(_loggerMock.Object, _weatherServiceMock.Object);
    }

    /// <summary>
    /// Validação para quando o país ou a capital não são fornecidos.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetWeatherForecast_ReturnsBadRequest_WhenCountryOrCapitalIsNull()
    {
        var result = await _weatherForecastController.GetWeatherForecast(null, "Brasília");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Validação para quando a previsão do tempo é retornada com sucesso.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetWeatherForecast_ReturnsOkResult_WhenWeatherInfoIsRetrieved()
    {
        var country = "Brasil";
        var capital = "Brasília";
        var weatherInfo = new WeatherInfo
        {
            Temperature = 30,
            Description = "Clear sky"
        };

        _weatherServiceMock.Setup(service => service.GetWeatherByCountryAndCapital(country, capital))
            .ReturnsAsync(weatherInfo);

        var result = await _weatherForecastController.GetWeatherForecast(country, capital);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedWeather = Assert.IsType<WeatherInfo>(okResult.Value);
        Assert.Equal(30, returnedWeather.Temperature);
        Assert.Equal("Clear sky", returnedWeather.Description);
    }

    /// <summary>
    /// Validação para quando ocorre uma exceção ao tentar obter as informações do tempo.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetWeatherForecast_Returns500_WhenExceptionIsThrown()
    {
        var country = "Brasil";
        var capital = "Brasília";

        _weatherServiceMock.Setup(service => service.GetWeatherByCountryAndCapital(country, capital))
            .ThrowsAsync(new Exception("API error"));

        var result = await _weatherForecastController.GetWeatherForecast(country, capital);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
