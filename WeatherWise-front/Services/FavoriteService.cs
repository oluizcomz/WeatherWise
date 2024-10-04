using Newtonsoft.Json.Linq;
using WeatherWise_front.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http;
using WeatherWise_front.Models;

namespace WeatherWise_front.Services
{
    public class FavoriteService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FavoriteService> _logger;
        private readonly Uri _apiEndpoint;

        public FavoriteService(HttpClient httpClient, ILogger<FavoriteService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (string.IsNullOrEmpty(configuration["urlApiEndpoint"]))
                throw new ArgumentNullException(nameof(configuration), "The Weather API Endpoint is missing from the configuration");

            _apiEndpoint = new Uri(configuration["urlApiEndpoint"], UriKind.Absolute);
        }

        /// <summary>
        /// Recupera uma lista de favoritos da API de favoritos com base no e-mail do usuário autenticado.
        /// </summary>
        /// <param name="email">O e-mail do usuário autenticado.</param>
        /// <param name="accessToken">O token de acesso fornecido pelo Auth0.</param>
        /// <returns>Uma lista de objetos <see cref="FavoriteModelView"/> representando os favoritos do usuário.</returns>
        /// <exception cref="Exception">Lança uma exceção se ocorrer um erro na comunicação com a API de favoritos.</exception>
        public async Task<List<Country>> GetFavorites(List<Country> countries, string email, string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email must be provided.", nameof(email));
                }

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new UnauthorizedAccessException("Access token is missing.");
                }

                var requestUri = new Uri(_apiEndpoint, $"Favorite?email={email}");
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var favoritesData = JArray.Parse(result);
                    var favorites = new List<FavoriteModelView>();

                    foreach (var favoriteData in favoritesData)
                    {
                        var favorite = new FavoriteModelView
                        {
                            Email = favoriteData["email"]?.ToString(),
                            CountryId = favoriteData["countryId"]?.ToString()
                        };

                        favorites.Add(favorite);
                    }

                    return countries
                        .Where(country => favorites.Any(fav => fav.CountryId == country.Alpha2Code))
                        .ToList();
                }
                else
                {
                    _logger.LogError("Error in GetFavorites: {StatusCode}", response.StatusCode);
                    throw new Exception("Error fetching favorites.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching GetFavorites.");
                throw;
            }
        }

        /// <summary>
        /// Adiciona um país aos favoritos do usuário autenticado enviando uma requisição POST.
        /// </summary>
        /// <param name="email">O e-mail do usuário autenticado.</param>
        /// <param name="countryId">O ID do país que será adicionado aos favoritos.</param>
        /// <param name="accessToken">O token de acesso fornecido pelo Auth0.</param>
        /// <returns>Uma string indicando o sucesso da operação.</returns>
        public async Task<string> AddFavorite(string email, string countryId, string accessToken)
        {
            try
            {
                var requestUri = new Uri(_apiEndpoint, $"Favorite?email={Uri.EscapeDataString(email)}&countryId={Uri.EscapeDataString(countryId)}");
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return "Favorite added successfully.";
                }
                else
                {
                    _logger.LogError("Error in AddFavorite: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException("Failed to add favorite.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding favorite.");
                throw;
            }
        }
        /// <summary>
        /// Remove um país dos favoritos do usuário.
        /// </summary>
        /// <param name="email">O email do usuário.</param>
        /// <param name="countryId">O código do país a ser removido.</param>
        /// <param name="accessToken">O token de acesso para autenticação.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        public async Task RemoveFavorite(string email, string countryId, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_apiEndpoint, $"Favorite?email={email}&countryId={countryId}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to remove favorite. Status code: {response.StatusCode}");
            }
        }
    }
}
