using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using apiPB.ApiClient.Abstraction;
using apiPB.Data;
using apiPB.Utils.Abstraction;

namespace apiPB.ApiClient.Implementation
{
    public class MagoApiClient : IMagoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogService _logService;

        public MagoApiClient(HttpClient httpClient, ApplicationDbContext dbContext, ILogService logService)
        {
            _dbContext = dbContext;
            _logService = logService;
            _baseUrl = GetConnectionString();

            // Configurazione JSON con camelCase
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false, // Per produzione, usa false per ridurre la dimensione
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            httpClient.BaseAddress = new Uri(_baseUrl);
            Console.WriteLine($"Base URL: {_baseUrl}");

            // Configurazione header di base
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("apiPB/1.0");

            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendPostAsyncWithToken<T>(string endpoint, IEnumerable<T> body, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            // Pulizia degli header per evitare conflitti
            request.Headers.Clear();
            
            // Header essenziali
            request.Headers.TryAddWithoutValidation("Authorization", token);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            
            // Header specifici per l'API Mago
            request.Headers.Host = "192.168.100.213";
            request.Headers.UserAgent.ParseAdd("apiPB/1.0");
            
            // Encoding supportati
            request.Headers.AcceptEncoding.Clear();
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            
            // Connessione
            request.Headers.Connection.Clear();
            request.Headers.Connection.Add("keep-alive");

            // Serializzazione con camelCase
            var jsonBody = JsonSerializer.Serialize(body, _jsonOptions);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Debug logging
            Console.WriteLine($"=== REQUEST DEBUG ===");
            Console.WriteLine($"Endpoint: {endpoint}");
            Console.WriteLine($"Method: {request.Method}");
            Console.WriteLine($"Request URI: {request.RequestUri}");
            Console.WriteLine($"Authorization Token: {token?.Substring(0, Math.Min(20, token.Length))}...");
            Console.WriteLine($"Request Body: {jsonBody}");
            Console.WriteLine($"Content-Type: {request.Content.Headers.ContentType}");
            Console.WriteLine($"Content-Length: {request.Content.Headers.ContentLength}");
            
            // Log degli header
            Console.WriteLine($"Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            
            if (request.Content?.Headers != null)
            {
                Console.WriteLine($"Content Headers:");
                foreach (var header in request.Content.Headers)
                {
                    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            Console.WriteLine($"=== END REQUEST DEBUG ===");

            try
            {
                var response = await _httpClient.SendAsync(request);
                
                // Debug della risposta
                Console.WriteLine($"=== RESPONSE DEBUG ===");
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Status Description: {response.ReasonPhrase}");
                
                // Leggi il contenuto della risposta per il debug
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");
                Console.WriteLine($"=== END RESPONSE DEBUG ===");
                
                // Riavvolgi il contenuto per permettere la lettura successiva
                if (!string.IsNullOrEmpty(responseContent))
                {
                    response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error in HTTP request: {ex.Message}\nstack trace: {ex.StackTrace}";
                _logService.AppendErrorToLog(errorMessage);
                throw;
            }
        }

        public async Task<HttpResponseMessage> SendPostAsync(string endpoint, object body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            
            // Header per il login
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serializzazione con camelCase
            var jsonBody = JsonSerializer.Serialize(body, _jsonOptions);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Console.WriteLine($"=== LOGIN REQUEST DEBUG ===");
            Console.WriteLine($"Endpoint: {endpoint}");
            Console.WriteLine($"Request Body: {jsonBody}");
            Console.WriteLine($"=== END LOGIN REQUEST DEBUG ===");

            try
            {
                var response = await _httpClient.SendAsync(request);
                
                Console.WriteLine($"=== LOGIN RESPONSE DEBUG ===");
                Console.WriteLine($"Status Code: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");
                Console.WriteLine($"=== END LOGIN RESPONSE DEBUG ===");
                
                // Riavvolgi il contenuto
                if (!string.IsNullOrEmpty(responseContent))
                {
                    response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in HTTP request: {ex.Message}\nstack trace: {ex.StackTrace}");
                throw;
            }
        }

        private string GetConnectionString()
        {
            var connectionString = _dbContext.A3AppSettings.FirstOrDefault()?.MagoUrl;
            if (connectionString != null) return connectionString;
            throw new InvalidOperationException("Connection string non trovata.");
        }
    }
}