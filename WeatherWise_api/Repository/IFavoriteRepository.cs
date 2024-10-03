using WeatherWise_api.Modal;

namespace WeatherWise_api.Repository
{
    public interface IFavoriteRepository
    {
        Task<List<Favorite>> GetFavorites(string email);
        Task AddFavorite(string email, string countryId);
        Task DeleteFavorite(string email, string countryId);
    }
}