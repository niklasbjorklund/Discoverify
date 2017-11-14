namespace Discoverify.ApiModels
{
    using Newtonsoft.Json;

    public class SearchArtistResponse
    {
        [JsonProperty("artists")]
        public SearchArtistCollection Artists { get; set; }
    }

}
