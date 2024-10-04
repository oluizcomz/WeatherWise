using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherWise_api.Modal;
using WeatherWise_api.Repository;
using WeatherWise_api.Services;

namespace WeatherWise_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecast(string country, string capital)
        {
            if (string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(capital))
            {
                return BadRequest("Country and capital must be provided.");
            }

            try
            {
                WeatherInfo weather = await _weatherService.GetWeatherByCountryAndCapital(country, capital);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather information.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
