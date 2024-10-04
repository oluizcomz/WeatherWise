using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherWise_front.Models;
using WeatherWise_front.Services;
using System.Security.Claims;
using System.Diagnostics;

namespace WeatherWise_front.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RestCountriesService _restCountriesService;
        private readonly FavoriteService _favoriteService;

        public HomeController(ILogger<HomeController> logger, RestCountriesService restCountriesService, FavoriteService favoriteService)
        {
            _logger = logger;
            _restCountriesService = restCountriesService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            List<Country> countries = await _restCountriesService.GetCountries();
            var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");
            var emailClaim = User.FindFirst(c => c.Type == ClaimTypes.Email);

            List<Country> favorites = new List<Country>();

            if (accessToken != null && emailClaim != null)
            {
                favorites = await _favoriteService.GetFavorites(countries, emailClaim.Value, accessToken);
            }

            var model = new HomeModelView
            {
                Countries = countries,
                Favorites = favorites
            };

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite(string countryId)
        {
            if (string.IsNullOrEmpty(countryId))
            {
                return Json(new { success = false, message = "Country ID must be provided." });
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "User email not found." });
                }

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Json(new { success = false, message = "Access token not found." });
                }

                await _favoriteService.AddFavorite(email, countryId, accessToken);

                return Json(new { success = true, message = "Country added to favorites successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding favorite.");
                return Json(new { success = false, message = "An error occurred while adding the country to favorites." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite(string countryId)
        {
            try
            {
                var emailClaim = User.FindFirst(c => c.Type == ClaimTypes.Email);
                if (emailClaim == null)
                {
                    return BadRequest("User email not found.");
                }

                var email = emailClaim.Value;

                var accessToken = await HttpContext.GetTokenAsync("Auth0", "access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized("Access token is missing.");
                }

                await _favoriteService.RemoveFavorite(email, countryId, accessToken);

                return Ok(new { message = "Favorite removed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing favorite.");
                return StatusCode(500, "An error occurred while removing the favorite.");
            }
        }


        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
