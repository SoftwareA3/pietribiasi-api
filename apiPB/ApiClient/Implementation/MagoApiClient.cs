using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using apiPB.ApiClient.Abstraction;
using apiPB.Data;

namespace apiPB.ApiClient.Implementation
{
    public class MagoApiClient : IMagoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _baseUrl;

        public MagoApiClient(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _baseUrl = GetConnectionString();

            httpClient.BaseAddress = new Uri(_baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendPostAsyncWithToken(string endpoint, object body, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            request.Headers.TryAddWithoutValidation("Authorization", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonBody = JsonSerializer.Serialize(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SendPostAsync(string endpoint, object body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonBody = JsonSerializer.Serialize(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(request);
        }

        private string GetConnectionString()
        {
            var connectionString = _dbContext.A3AppSettings.FirstOrDefault()?.MagoUrl;
            if (connectionString != null) return connectionString;
            throw new InvalidOperationException("Connection string non trovata.");
        }
    }
}