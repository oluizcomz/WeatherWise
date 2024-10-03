using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherWise_api.Modal;
using WeatherWise_api.Services;

namespace WeatherWise_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherService _weatherService;

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

        /*[HttpGet(Name = "GetWeatherForecast")]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/
    }
}
