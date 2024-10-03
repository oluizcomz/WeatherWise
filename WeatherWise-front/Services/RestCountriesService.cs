using Newtonsoft.Json.Linq;
using WeatherWise_front.Models;

namespace WeatherWise_front.Services
{
    public class RestCountriesService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestCountriesService> _logger;

        public RestCountriesService(HttpClient httpClient, ILogger<RestCountriesService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        /// <summary>
        /// Recupera uma lista de países a partir da API REST Countries.
        /// </summary>
        /// <returns>
        /// Uma lista de objetos <see cref="Country"/> representando os países.
        /// </returns>
        /// <exception cref="Exception">
        /// Lança uma exceção se ocorrer um erro ao buscar os dados dos países ou se a resposta da API não for bem-sucedida.
        /// </exception>
        public async Task<List<Country>> GetCountries()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all");

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var countriesData = JArray.Parse(result);
                    var countries = new List<Country>();

                    foreach (var countryData in countriesData)
                    {
                        var country = new Country
                        {
                            //Name = countryData["name"]?["common"]?.ToString(),
                            Name = countryData["translations"]?["por"]?["common"]?.ToString()
                                   ?? countryData["name"]?["common"]?.ToString(),
                            Alpha2Code = countryData["cca2"]?.ToString(),
                            Alpha3Code = countryData["cca3"]?.ToString(),
                            Capital = countryData["capital"]?.ToString(),
                            Population = countryData["population"]?.ToObject<long>() ?? 0,
                            Languages = countryData["languages"]?.ToObject<Dictionary<string, string>>()?.Values.ToArray(),
                            Currencies = countryData["currencies"]?.ToObject<Dictionary<string, JObject>>()?.Values.Select(c => c["name"]?.ToString()).ToArray()
                        };

                        countries.Add(country);
                    }

                    return countries.OrderBy(c => c.Name).ToList(); ;
                }
                else
                {
                    _logger.LogError("Error in GetCountriesAsync: {StatusCode}", response.StatusCode);
                    throw new Exception("Error fetching in GetCountries.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching GetCountries.");
                throw;
            }
        }
    }
}
