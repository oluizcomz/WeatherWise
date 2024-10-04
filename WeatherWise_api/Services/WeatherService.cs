using Newtonsoft.Json.Linq;
using WeatherWise_api.Modal;

namespace WeatherWise_api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private string _apiKey;
        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = Environment.GetEnvironmentVariable("ApiKeyOW");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("A chave da API não foi encontrada. Verifique se a variável de ambiente ApiKeyOW está definida.");
            }
        }

        public async Task<WeatherInfo> GetWeatherByCountryAndCapital(string country, string capital)
        {
            try
            {
                string requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q={capital},{country}&appid={_apiKey}&lang=pt&units=metric";

                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var weatherData = JObject.Parse(result);

                    var weatherInfo = new WeatherInfo
                    {
                        Temperature = weatherData["main"]?["temp"]?.ToObject<double>() ?? 0,
                        Description = weatherData["weather"]?[0]?["description"]?.ToString(),
                    };

                    return weatherInfo;
                }
                else
                {
                    _logger.LogError("Error fetching weather data: {StatusCode}", response.StatusCode);
                    throw new Exception("Error fetching weather data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching weather data.");
                throw;
            }
        }
    }
}
