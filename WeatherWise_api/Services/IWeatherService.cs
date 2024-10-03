using WeatherWise_api.Modal;

namespace WeatherWise_api.Services
{
    public interface IWeatherService
    {
        Task<WeatherInfo> GetWeatherByCountryAndCapital(string country, string capital);
    }
}