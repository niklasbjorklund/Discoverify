namespace Discoverify.ApiModels
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SearchArtistCollection
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("items")]
        public IList<Artist> Items { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
