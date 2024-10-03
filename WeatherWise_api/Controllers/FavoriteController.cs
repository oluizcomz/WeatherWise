using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherWise_api.Repository;

namespace WeatherWise_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteController : ControllerBase
    {
       

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteController(ILogger<WeatherForecastController> logger, IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
            _logger = logger;
        }

        [HttpGet(Name = "Favorite")]
        //[Authorize]
        public async Task<IActionResult> GetFavorite(string email){
            //string site = Request.Headers["Origin"].ToString();
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("email must be provided.");
            }
            try{
                var favorites = await _favoriteRepository.GetFavorites(email);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get favorites.");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost(Name = "Favorite")]
        //[Authorize]
        public async Task<IActionResult> AddFavorite(string email, string countryId){
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(countryId))
            {
                return BadRequest("email, countryID must be provided.");
            }
            try
            {
                await _favoriteRepository.AddFavorite(email, countryId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add favorite.");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpDelete(Name = "Favorite")]
        //[Authorize]
        public async Task<IActionResult> DeleteFavorite(string email, string countryId){
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(countryId) )
            {
                return BadRequest("email, country and capital must be provided.");
            }
            try
            {
                await _favoriteRepository.DeleteFavorite(email, countryId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete favorite.");
                return StatusCode(500, "Internal server error.");
            }
        } 
    }
}
