using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using NewsAggregatorLambda.Models;

namespace NewsAggregatorLambda
{
    public class NewsService : INewsService
    {
        private readonly string _apiKey;
        private readonly ILambdaLogger _logger;
        private readonly HttpClient _httpClient;

        public NewsService(string apiKey, ILambdaLogger logger)
        {
            _apiKey = apiKey;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<NewsResponse> GetTopHeadlinesAsync()
        {
            var url = $"https://api.currentsapi.services/v1/latest-news?apiKey={_apiKey}";
            _logger.LogLine($"Fetching news from: {url}");
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var newsResponse = await response.Content.ReadFromJsonAsync<NewsResponse>();
            return newsResponse ?? new NewsResponse();
        }
    }

    public class NewsController
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task FetchAndDisplayNews()
        {
            var news = await _newsService.GetTopHeadlinesAsync();
            // Process and display news
        }
    }
}