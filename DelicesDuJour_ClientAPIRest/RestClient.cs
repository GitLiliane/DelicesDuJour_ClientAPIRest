using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest
{
    internal class RestClient
    {
        private static readonly object _lock = new object();
        private static RestClient _instance;
        public static RestClient Instance
        {
            get
            {
                if(_instance is null)
                {
                    //plusieurs thread
                    lock(_lock) //multithreading
                    {
                        //1 seul thread à la fois
                        if (_instance is null)
                        {
                            _instance = new RestClient();
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly HttpClient _httpClient = new();

        public string BaseUrl { get; set; }

        public string JwtToken { get; set; }

        #region GET
        private RestClient() { }

        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Get, endpoint, null, customHeaders);

            // Si la réponse est vide (NoContent ou Content-Length = 0 ou null), retourne default
            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        #endregion GET

        #region POST

        public async Task<T> PostAsync<T, C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Post, endpoint, JsonContent.Create(content), customHeaders);

            // Si la réponse est vide (NoContent ou Content-Length = 0 ou null), retourne default
            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        public async Task PostAsync<C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Post, endpoint, JsonContent.Create(content), customHeaders);
        }

        public async Task PostAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Post, endpoint, null, customHeaders);
        }

        #endregion POST

        #region PUT

        public async Task<T> PutAsync<T, C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Put, endpoint, JsonContent.Create(content), customHeaders);

            // Si la réponse est vide (NoContent ou Content-Length = 0 ou null), retourne default
            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        public async Task PutAsync<C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Put, endpoint, JsonContent.Create(content), customHeaders);
        }

        public async Task PutAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Put, endpoint, null, customHeaders);
        }

        #endregion PUT

        #region DELETE

        public async Task<T> DeleteAsync<T>(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Delete, endpoint, null, customHeaders);

            // Si la réponse est vide (NoContent ou Content-Length = 0 ou null), retourne default
            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        public async Task DeleteAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Delete, endpoint, null, customHeaders);
        }

        #endregion DELETE

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, HttpContent content = null, Dictionary<string, string> customHeaders = null)
        {
            // Création de la requête Http
            HttpRequestMessage httpRequest;
            try
            {
                endpoint = CombineUrl(BaseUrl, endpoint);
                httpRequest = new HttpRequestMessage(method, endpoint) { Content = content };
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpRequest.Headers.Authorization = string.IsNullOrWhiteSpace(JwtToken) ? null : new AuthenticationHeaderValue("Bearer", JwtToken);
                if (customHeaders != null)
                {
                    foreach (var header in customHeaders)
                    {
                        httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
            catch (Exception)
            {
                throw new RestClientException($"Erreur de création de la requête Http '{endpoint}'");
            }

            // Exécution de la requête Http
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequest);
            }
            catch (Exception)
            {
                throw new RestClientException($"Erreur d'accès à l'API '{endpoint}'");
            }

            // Lecture du résultat de la requête Http
            if (!response.IsSuccessStatusCode)
            {
                // On lit le contenu brut (JSON ou autre)
                var rawContent = await response.Content.ReadAsStringAsync();

                var message = $"Erreur retournée par l'API ({(int)response.StatusCode}-{response.ReasonPhrase})";
                throw new RestClientException(message, rawContent);
            }

            return response;
        }

        private string CombineUrl(string baseUrl, string endpoint)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return endpoint;
            if (string.IsNullOrEmpty(endpoint))
                return baseUrl;
            return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }

    public class RestClientException : Exception
    {
        static readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

        private string _rawContent;

        public RestClientException(string message) : base(message) { }

        public RestClientException(string message, string rawContent) : base(message)
        {
            _rawContent = rawContent;
        }

        public bool HasRawContent => !string.IsNullOrWhiteSpace(_rawContent);

        public string GetRawContent() => _rawContent;

        public bool GetRawContent<T>(out T content)
        {
            content = default;

            try
            {
                content = JsonSerializer.Deserialize<T>(_rawContent, serializerOptions);
                return true;
            }
            catch (Exception) { }

            return false;
        }
    }

    public static class HttpContentExtensions
    {
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<T> ReadJsonSafeAsync<T>(this HttpContent content)
        {
            try
            {
                return await content.ReadFromJsonAsync<T>(serializerOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}


