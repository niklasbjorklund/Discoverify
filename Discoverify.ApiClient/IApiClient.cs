namespace Discoverify.ApiClient
{
    using Discoverify.ApiModels;
    using System.Threading.Tasks;

    public interface IApiClient
    {
        Task<SearchArtistResponse> SearchArtistsAsync(string artistName, int limit, int offset);
        Task<GenreCollection> GetAvailableGenreSeeds();
        Task<RecommendationRepsonse> GetRecomendations(string genre, int limit);
    }
}
