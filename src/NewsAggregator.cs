#nullable enable
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using CurrentNewsAPI.Services;
using NewsAggregatorLambda;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CurrentNewsAPI.Functions
{
    public class NewsAggregatorFunction
    {
        private readonly IHttpClientWrapper? _httpClient;

        // Parameterless constructor required by AWS Lambda
        public NewsAggregatorFunction()
        {
            _httpClient = null;
        }

        // Constructor for dependency injection
        public NewsAggregatorFunction(IHttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            try
            {
                // Initialize httpClient if not injected
                var httpClient = _httpClient ?? new HttpClientWrapper(context.Logger);
                
                // Get API key from Secrets Manager
                var apiKey = await SecretsManager.GetCurrentsApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    context.Logger.LogLine("ERROR: Failed to retrieve API key from Secrets Manager.");
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 500,
                        Body = JsonSerializer.Serialize(new { error = "API key not configured" })
                    };
                }

                // Update this line to match the actual NewsService constructor
                var newsService = new NewsService(apiKey, context.Logger);
                
                context.Logger.LogLine("Fetching top headlines...");
                var news = await newsService.GetTopHeadlinesAsync();

                if (news?.News == null || news.News.Count == 0)
                {
                    context.Logger.LogLine("WARNING: No news articles found.");
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = JsonSerializer.Serialize(new { message = "No news articles found" })
                    };
                }

                context.Logger.LogLine($"Successfully retrieved {news.News.Count} articles:");
                foreach (var article in news.News)
                {
                    context.Logger.LogLine($"- {article.Title} - {article.Url}");
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    },
                    Body = JsonSerializer.Serialize(news)
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"ERROR: An unhandled exception occurred: {ex.Message}");
                context.Logger.LogLine($"Stack trace: {ex.StackTrace}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonSerializer.Serialize(new { error = ex.Message })
                };
            }
        }
    }
}