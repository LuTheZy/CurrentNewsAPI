using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using System.Net;
using Amazon.Lambda.Core;

namespace CurrentNewsAPI.Services
{
    public class HttpClientWrapper : IHttpClientWrapper, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILambdaLogger _logger;
        private readonly IAsyncPolicy<HttpResponseMessage> _resilientPolicy;
        private bool _disposed;

        public HttpClientWrapper(ILambdaLogger logger)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _logger = logger;
            
            var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => 
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        var error = exception.Exception?.Message ?? $"HTTP {exception.Result?.StatusCode}";
                        _logger.LogLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {error}");
                    });

            var circuitBreaker = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, duration) => 
                        _logger.LogLine($"Circuit breaker opened for {duration.TotalSeconds} seconds due to: {ex.Exception?.Message}"),
                    onReset: () => 
                        _logger.LogLine("Circuit breaker reset"),
                    onHalfOpen: () => 
                        _logger.LogLine("Circuit breaker half-open"));

            _resilientPolicy = Policy.WrapAsync(retryPolicy, circuitBreaker);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _resilientPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(url));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}

