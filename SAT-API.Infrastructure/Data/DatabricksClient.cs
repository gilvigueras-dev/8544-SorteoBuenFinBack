using System.Net.Http.Headers;

namespace SAT_API.Infrastructure.Data
{
    public class DatabricksClient : IDatabricksClient
    {
        private readonly HttpClient _httpClient;

            public DatabricksClient(HttpClient httpClient, string instanceURL, string accessToken, EnvironmentService environmentService)
            {
                _httpClient = httpClient;

                if (string.IsNullOrEmpty(instanceURL))
                {
                    instanceURL = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DATABRICKS-HOST");
                }

                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DATABRICKS-TOKEN");
                }

                // Configurar headers de autenticación
                _httpClient.BaseAddress = new Uri($"https://{instanceURL}");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Trim());
                _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DatabricksClient", "1.0"));
            }

        public HttpClient GetClient()
        {
            return _httpClient;
        }
    }
}
