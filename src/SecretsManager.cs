using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace CurrentNewsAPI.Services
{
    public static class SecretsManager
    {
        private static readonly string SecretName = "currents/news/api";
        private static readonly string Region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";

        public static async Task<string> GetCurrentsApiKeyAsync()
        {
            try
            {
                var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(Region));
                var request = new GetSecretValueRequest { SecretId = SecretName };
                var response = await client.GetSecretValueAsync(request);
                var secretJson = response.SecretString;
                var secretDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secretJson);
                return secretDict?["CURRENTS_API_KEY"] ?? throw new InvalidOperationException("API key not found in secret");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve API key from Secrets Manager: {ex.Message}", ex);
            }
        }
    }
} 