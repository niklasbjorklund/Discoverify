namespace Discoverify.ApiModels
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class GenreCollection
    {
        [JsonProperty("genres")]        
        public IList<string> Genres { get; set; }
    }
}
