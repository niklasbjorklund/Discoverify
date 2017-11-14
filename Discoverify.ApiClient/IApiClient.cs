namespace Discoverify.ApiClient
{
    using Discoverify.ApiModels;
    using System.Threading.Tasks;

    public interface IApiClient
    {
        Task<SearchArtistResponse> SearchArtistsAsync(string artistName, int? limit = null, int? offset = null);
        Task<GenreCollection> GetAvailableGenreSeeds();
        Task<RecommendationRepsonse> GetRecomendations(string genre, string artistId = null, string trackId = null);
    }
}
