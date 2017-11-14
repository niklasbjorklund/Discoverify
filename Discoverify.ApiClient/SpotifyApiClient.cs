namespace Discoverify.ApiClient
{
    using Discoverify.ApiModels;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class SpotifyApiClient : IApiClient
    {
        private const string ClientId = "480b88b7f626403dab28e9697ca0fcb2";
        private const string ClientSecret = "779f5fe00bca4ea08f2ecd1a1fbe0ebf";

        protected const string BaseUrl = "https://api.spotify.com/";

        private HttpClient GetDefaultClient()
        {
            var authHandler = new SpotifyAuthClientCredentialsHttpMessageHandler(
                ClientId,
                ClientSecret,
                new HttpClientHandler());

            var client = new HttpClient(authHandler)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            return client;
        }

        public async Task<SearchArtistResponse> SearchArtistsAsync(string artistName, int? limit = null, int? offset = null)
        {
            var client = GetDefaultClient();
            var url = $"/v1/search?q={artistName}&type=artist&limit={limit}&offset={offset}";

            var response = await client.GetStringAsync(url);

            return JsonConvert.DeserializeObject<SearchArtistResponse>(response);
        }

        public async Task<GenreCollection> GetAvailableGenreSeeds()
        {
            var client = GetDefaultClient();
            var url = $"v1/recommendations/available-genre-seeds";

            var response = await client.GetStringAsync(url);

            return JsonConvert.DeserializeObject<GenreCollection>(response);
        }

        public async Task<RecommendationRepsonse> GetRecomendations(string genre, string artistId = null, string trackId = null)
        {
            var client = GetDefaultClient();
            var url = $"v1/recommendations?seed_genres={genre}&limit=10";

            var response = await client.GetStringAsync(url);

            return JsonConvert.DeserializeObject<RecommendationRepsonse>(response);
        }
    }
}
