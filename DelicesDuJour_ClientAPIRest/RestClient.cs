using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Namespace principal du client REST de l'application.
// Il contient la classe RestClient et les classes associées.
namespace DelicesDuJour_ClientAPIRest
{
    // Classe interne (non accessible depuis l'extérieur du projet)
    // gérant toutes les interactions avec l’API REST.
    // Elle est conçue comme un singleton pour garantir une seule instance partagée.
    internal class RestClient
    {
        // 🔒 Objet de verrouillage utilisé pour sécuriser la création du singleton
        // en cas d'accès concurrent (multi-threading).
        private static readonly object _lock = new object();

        // Instance unique du RestClient.
        private static RestClient _instance;

        // Propriété publique d'accès à l'instance unique.
        // Elle implémente le design pattern "Singleton" avec double vérification.
        public static RestClient Instance
        {
            get
            {
                if (_instance is null)
                {
                    // Plusieurs threads peuvent arriver ici simultanément.
                    lock (_lock) // verrou pour s’assurer qu’un seul thread crée l’instance
                    {
                        // Vérifie à nouveau que l’instance n’a pas été créée entre-temps.
                        if (_instance is null)
                        {
                            _instance = new RestClient();
                        }
                    }
                }

                return _instance;
            }
        }

        // ✅ HttpClient unique et réutilisé pour toutes les requêtes
        // (bonne pratique pour éviter les fuites de sockets).
        private static readonly HttpClient _httpClient = new();

        // URL de base de l'API (par ex : "https://api.monsite.com")
        public string BaseUrl { get; set; }

        // Jeton JWT utilisé pour l'authentification (Authorization: Bearer ...)
        public string JwtToken { get; set; }

        // 🛠️ Constructeur privé : empêche la création directe d’instances.
        private RestClient()
        {
            // Définit un délai maximum de 10 minutes pour les requêtes HTTP.
            _httpClient.Timeout = TimeSpan.FromMinutes(10);

            // Définit un en-tête "User-Agent" pour identifier le client côté serveur.
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DelicesDuJour-Client/1.0");
        }

        // ========================
        #region GET     
        // ========================

        // Méthode générique pour effectuer une requête HTTP GET.
        // T = type de la donnée à désérialiser (ex: List<CategorieDTO>).
        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Get, endpoint, null, customHeaders);

            // Si la réponse est vide (204 No Content ou sans corps), retourne default(T)
            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            // Lecture et désérialisation sécurisée du contenu JSON.
            return await response.Content.ReadJsonSafeAsync<T>();
        }

        #endregion GET

        // ========================
        #region POST
        // ========================

        // Envoie une requête POST avec un corps JSON et retourne une réponse typée.
        public async Task<T> PostAsync<T, C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Post, endpoint, JsonContent.Create(content), customHeaders);

            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        // Variante : envoie un POST sans attendre de retour (void).
        public async Task PostAsync<C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Post, endpoint, JsonContent.Create(content), customHeaders);
        }

        // Variante : POST sans contenu (par exemple déclenchement d’une action serveur).
        public async Task PostAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Post, endpoint, null, customHeaders);
        }

        // Envoie un POST multipart (ex : upload de fichier ou formulaire).
        public async Task<T> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent multipartContent)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(JwtToken))
                headers["Authorization"] = $"Bearer {JwtToken}";

            var response = await SendAsync(HttpMethod.Post, endpoint, multipartContent, headers);
            return await response.Content.ReadJsonSafeAsync<T>();
        }

        #endregion POST

        // ========================
        #region PUT
        // ========================

        // Méthode PUT avec corps JSON et réponse typée.
        public async Task<T> PutAsync<T, C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Put, endpoint, JsonContent.Create(content), customHeaders);

            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        // PUT sans valeur de retour.
        public async Task PutAsync<C>(string endpoint, C content, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Put, endpoint, JsonContent.Create(content), customHeaders);
        }

        // PUT sans corps (rare, mais possible pour certaines APIs).
        public async Task PutAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Put, endpoint, null, customHeaders);
        }

        // PUT multipart (ex : modification d'une image).
        public async Task<T> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent multipartContent)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(JwtToken))
                headers["Authorization"] = $"Bearer {JwtToken}";

            var response = await SendAsync(HttpMethod.Put, endpoint, multipartContent, headers);
            return await response.Content.ReadJsonSafeAsync<T>();
        }

        #endregion PUT

        // ========================
        #region DELETE
        // ========================

        // Méthode DELETE avec retour typé (souvent inutile, mais possible).
        public async Task<T> DeleteAsync<T>(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            HttpResponseMessage response = await SendAsync(HttpMethod.Delete, endpoint, null, customHeaders);

            var contentLength = response.Content?.Headers?.ContentLength;
            if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0 || contentLength is null)
                return default;

            return await response.Content.ReadJsonSafeAsync<T>();
        }

        // Méthode DELETE simple (sans retour).
        public async Task DeleteAsync(string endpoint, Dictionary<string, string> customHeaders = null)
        {
            await SendAsync(HttpMethod.Delete, endpoint, null, customHeaders);
        }

        #endregion DELETE

        // ========================
        // Méthode centrale : exécute réellement la requête HTTP.
        // ========================
        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, HttpContent content = null, Dictionary<string, string> customHeaders = null)
        {
            HttpRequestMessage httpRequest;
            try
            {
                // Concatène la BaseUrl et le endpoint pour former l’URL complète.
                endpoint = CombineUrl(BaseUrl, endpoint);

                // Crée la requête HTTP.
                httpRequest = new HttpRequestMessage(method, endpoint) { Content = content };
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpRequest.Headers.Authorization = string.IsNullOrWhiteSpace(JwtToken) ? null : new AuthenticationHeaderValue("Bearer", JwtToken);

                // Ajoute d’éventuels en-têtes personnalisés.
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

            HttpResponseMessage response;
            try
            {
                // Exécution réelle de la requête via HttpClient.
                response = await _httpClient.SendAsync(httpRequest);
            }
            catch (Exception)
            {
                throw new RestClientException($"Erreur d'accès à l'API '{endpoint}'");
            }

            // Vérifie que la requête s’est bien déroulée (status code 2xx).
            if (!response.IsSuccessStatusCode)
            {
                var rawContent = await response.Content.ReadAsStringAsync();
                var message = $"Erreur retournée par l'API ({(int)response.StatusCode}-{response.ReasonPhrase})";
                throw new RestClientException(message, rawContent);
            }

            return response;
        }

        // Combine la BaseUrl et le endpoint en évitant les doubles /.
        private string CombineUrl(string baseUrl, string endpoint)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return endpoint;
            if (string.IsNullOrEmpty(endpoint))
                return baseUrl;
            return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }

    // =====================================================
    // Classe d'exception personnalisée pour le RestClient.
    // Elle permet de conserver le message d’erreur et le contenu brut renvoyé par l’API.
    // =====================================================
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

        // Essaie de désérialiser le contenu brut (_rawContent) dans un objet typé.
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

    // =====================================================
    // Classe d’extension pour lire du JSON en toute sécurité
    // sans faire planter l’application en cas d’erreur de format.
    // =====================================================
    public static class HttpContentExtensions
    {
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };

        // Méthode d’extension pour désérialiser le contenu JSON d’une réponse HTTP.
        // Retourne default(T) si le JSON est invalide.
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
