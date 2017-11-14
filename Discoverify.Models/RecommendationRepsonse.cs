namespace Discoverify.ApiModels
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class RecommendationRepsonse
    {
        [JsonProperty("tracks")]
        public IEnumerable<RecommendationCollection> Tracks { get; set; }
    }
}
