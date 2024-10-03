
using Amazon.DynamoDBv2.DataModel;
using WeatherWise_api.Modal;

namespace WeatherWise_api.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ILogger<FavoriteRepository> _logger;
        private readonly IDynamoDBContext _dynamoDBContext;
        public FavoriteRepository(ILogger<FavoriteRepository> logger, IDynamoDBContext dynamoDBContext)
        {
            _logger = logger;
            _dynamoDBContext = dynamoDBContext;
        }

        public async Task AddFavorite(string email, string countryId)
        {
            Favorite favorite = new Favorite { Email = email, CountryId = countryId };
            await _dynamoDBContext.SaveAsync(favorite);
        }

        public async Task DeleteFavorite(string email, string countryId)
        {
            Favorite favorite = new Favorite { Email = email, CountryId = countryId };
            await _dynamoDBContext.DeleteAsync(favorite);

        }

        public async Task<List<Favorite>> GetFavorites(string email)
        {
            List<Favorite> favorites = await _dynamoDBContext.QueryAsync<Favorite>(email).GetRemainingAsync();
            return favorites;
        }
    }
}