using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NewsAggregatorLambda.Models
{
    public class NewsResponse
    {
        [JsonPropertyName("news")]
        public List<Article> News { get; set; } = new List<Article>();
    }

    public class Article
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("published")]
        public string Published { get; set; } = "";

        [JsonPropertyName("author")]
        public string Author { get; set; } = "";
    }
}