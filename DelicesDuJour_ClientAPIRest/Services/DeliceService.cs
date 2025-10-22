using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace DelicesDuJour_ClientAPIRest.Services
{
    internal class DeliceService
    {
        // ----------------------- URL des API -----------------------
        // Endpoints pour l'authentification
        const string URL_POST_LOGIN = "/api/Authentication/Login/";

        // Endpoints pour les recettes
        const string URL_GET_RECETTES = "api/Recettes";
        const string URL_GET_RECETTE_BY_ID = "api/Recettes";
        const string URL_GET_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations";
        const string URL_POST_RECETTE = "api/Recettes";
        const string URL_PUT_RECETTE = "api/Recettes";
        const string URL_DELETE_RECETTE = "api/Recettes";

        // Endpoints pour les catégories
        const string URL_GET_CATEGORIES = "api/Categories";
        const string URL_POST_CATEGORIES = "api/Categories";
        const string URL_PUT_CATEGORIES = "api/Categories";
        const string URL_DELETE_CATEGORIES = "api/Categories";

        // Endpoints pour les relations recettes-catégories
        const string URL_GET_RECETTES_BY_IDCATEGORIE = "api/RecettesCategoriesRelations/GetRecettesByIdCategorie";
        const string URL_GET_CATEGORIES_BY_RECETTE = "api/RecettesCategoriesRelations/GetCategoriesByIdRecette";
        const string URL_POST_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations/AddRecetteCategorieRelationship";
        const string URL_REMOVE_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations/RemoveRecetteCategorieRelationship";

        // Singleton RestClient utilisé pour les requêtes API
        private readonly RestClient _rest = RestClient.Instance;

        #region Singleton
        // ----------------------- Singleton DeliceService -----------------------
        private static readonly object _lock = new object(); // verrou pour le multithreading
        private static DeliceService _instance;

        public static DeliceService Instance
        {
            get
            {
                if (_instance is null)
                {
                    // Protection multithread
                    lock (_lock)
                    {
                        if (_instance is null)
                        {
                            // Création unique de l'instance
                            _instance = new DeliceService();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion
        #region Login
        // ----------------------- Login -----------------------
        public async Task<bool> Login(string baseurl, LoginDTO loginDTO)
        {
            _rest.BaseUrl = baseurl;

            // Envoi de la requête POST avec les informations de connexion
            var res = await _rest.PostAsync<JwtDTO, LoginDTO>(URL_POST_LOGIN, loginDTO);

            if (res != null)
            {
                // Sauvegarde du token JWT dans le RestClient pour les futures requêtes
                _rest.JwtToken = res.Token;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Récupère les rôles depuis le JWT
        public IEnumerable<string> GetRolesFromJwt(string[] possibleClaimTypes = null)
        {
            // Si aucun claim type fourni, on utilise ceux standards
            possibleClaimTypes ??=
            [
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        "role",
        "roles"
            ];

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(_rest.JwtToken);

                // Filtrage des claims correspondant aux rôles
                var roles = jwtToken.Claims
                    .Where(c => possibleClaimTypes.Contains(c.Type))
                    .Select(c => c.Value);

                return roles;
            }
            catch (Exception ex)
            {
                // Gestion d'erreur si le JWT est invalide
                throw new InvalidOperationException($"Erreur lors du décodage du JWT : {ex.Message}");
            }
        }

        // Vérifie si l'utilisateur est connecté (token JWT présent)
        public bool IsConnected()
        {
            return !string.IsNullOrEmpty(_rest.JwtToken);
        }

        // Déconnexion : supprime le token
        public void Logout()
        {
            _rest.JwtToken = null;
        }
        #endregion Fin Login

        #region Recette

        // Récupère toutes les recettes depuis l'API
        public async Task<IEnumerable<RecetteDTO>> GetRecettesAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteDTO>>($"{URL_GET_RECETTES}");
            return res;
        }

        // Récupère une recette par son Id
        public async Task<RecetteDTO> GetRecetteByIdAsync(int idRecette)
        {
            try
            {
                var res = await _rest.GetAsync<RecetteDTO>($"{URL_GET_RECETTE_BY_ID}/{idRecette}");
                return res;
            }
            catch (Exception e)
            {
                // Gestion d'erreur si la recette n'est pas récupérable
                throw new Exception("Impossible de récupérer la recette");
            }
        }

        // Récupère toutes les recettes appartenant à une catégorie donnée
        public async Task<IEnumerable<RecetteDTO>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteDTO>>(
                    $"{URL_GET_RECETTES_BY_IDCATEGORIE}/{idCategorie}");
            return res;
        }

        #endregion Fin Recette

        #region Catégories

        // Récupère toutes les catégories depuis l'API
        public async Task<IEnumerable<CategorieDTO>> GetCategoriesAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<CategorieDTO>>($"{URL_GET_CATEGORIES}");
            return res;
        }

        // Ajoute une nouvelle catégorie
        public async Task<CategorieDTO> AddCategorieAsync(CreateCategorieDTO createDTO)
        {
            var res = await _rest.PostAsync<CategorieDTO, CreateCategorieDTO>($"{URL_POST_CATEGORIES}", createDTO);
            return res;
        }

        // Met à jour une catégorie existante par son Id
        public async Task<CategorieDTO> UpdateCategorieAsync(UpdateCategorieDTO updateDTO, int id)
        {
            var res = await _rest.PutAsync<CategorieDTO, UpdateCategorieDTO>($"{URL_PUT_CATEGORIES}/{id}", updateDTO);
            return res;
        }

        // Supprime une catégorie par son Id
        public async Task DeleteCategorieAsync(int id)
        {
            await _rest.DeleteAsync($"{URL_DELETE_CATEGORIES}/{id}");
        }

        #endregion Fin Catégories

        #region Relation Recettes Catégories

        // Récupère les catégories associées à une recette
        public async Task<IEnumerable<CategorieDTO>> GetCategoriesByIdRecette(int id)
        {
            var res = await _rest.GetAsync<IEnumerable<CategorieDTO>>(
                    $"{URL_GET_CATEGORIES_BY_RECETTE}/{id}");
            return res;
        }

        // Crée une relation entre une recette et une catégorie
        public async Task AddRelationRecetteCategorieAsync(int idCategorie, int idRecette)
        {
            await _rest.PostAsync($"{URL_POST_RECETTES_CATEGORIES_RELATIONS}/{idCategorie}/{idRecette}");
        }

        // Supprime une relation entre une recette et une catégorie
        public async Task DeleteRelationRecetteCategorieAsync(int idCategorie, int idRecette)
        {
            await _rest.DeleteAsync($"{URL_REMOVE_RECETTES_CATEGORIES_RELATIONS}/{idCategorie}/{idRecette}");
        }

        // Récupère toutes les relations entre recettes et catégories
        public async Task<IEnumerable<RecetteCategorieRelationshipDTO>> GetRecetteCategorieRelationshipsAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteCategorieRelationshipDTO>>(URL_GET_RECETTES_CATEGORIES_RELATIONS);
            return res;
        }

        #endregion Fin Relation Recettes Catégories


        #region Gestion Recette

        // Crée une nouvelle recette, éventuellement avec une image
        public async Task<RecetteDTO> CreateRecetteAsync(CreateRecetteDTO dto, string imagePath = null)
        {
            // Cas sans image : envoi simple via POST
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                var res = await _rest.PostAsync<RecetteDTO, CreateRecetteDTO>(URL_POST_RECETTE, dto);
                return res;
            }

            // Vérification du token JWT avant envoi multipart
            if (string.IsNullOrWhiteSpace(RestClient.Instance.JwtToken))
                throw new InvalidOperationException("Le token JWT n'est pas défini. Connectez-vous d'abord.");

            string url = $"{RestClient.Instance.BaseUrl}/api/recettes";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", RestClient.Instance.JwtToken);

            using var multipart = new MultipartFormDataContent();

            // Sérialiser le DTO en JSON
            var dtoJson = JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var jsonContent = new StringContent(dtoJson, Encoding.UTF8, "application/json");
            multipart.Add(jsonContent, "request"); // Le nom "request" doit correspondre au paramètre côté API

            // Ajouter le fichier image
            var fileStream = File.OpenRead(imagePath);
            var fileContent = new StreamContent(fileStream);
            var mime = MimeMappingFromExtension(Path.GetExtension(imagePath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);
            multipart.Add(fileContent, "photoFile", Path.GetFileName(imagePath)); // Le nom "photoFile" côté API

            // Envoyer la requête
            var response = await httpClient.PostAsync(url, multipart);
            response.EnsureSuccessStatusCode();

            // Désérialiser la réponse en RecetteDTO
            var responseJson = await response.Content.ReadAsStringAsync();
            var recette = JsonSerializer.Deserialize<RecetteDTO>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return recette!;
        }

        // Helper minimal pour déduire le type MIME à partir de l'extension de fichier
        private static string MimeMappingFromExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return "application/octet-stream";
            ext = ext.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        // Met à jour une recette existante, éventuellement avec une image
        public async Task<RecetteDTO> UpdateRecetteAsync(UpdateRecetteDTO updateRecette, string imagePath = null)
        {
            // Cas sans image : PUT simple
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                var res = await _rest.PutAsync<RecetteDTO, UpdateRecetteDTO>(
                    $"{URL_PUT_RECETTE}/{updateRecette.Id}",
                    updateRecette
                );
                return res;
            }

            // Vérification du token JWT
            if (string.IsNullOrWhiteSpace(RestClient.Instance.JwtToken))
                throw new InvalidOperationException("Le token JWT n'est pas défini. Connectez-vous d'abord.");

            // URL incluant l'ID pour le controller
            string url = $"{RestClient.Instance.BaseUrl}/api/recettes/{updateRecette.Id}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", RestClient.Instance.JwtToken);

            using var multipart = new MultipartFormDataContent();

            // Sérialisation du DTO en JSON
            var dtoJson = JsonSerializer.Serialize(updateRecette, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var jsonContent = new StringContent(dtoJson, Encoding.UTF8, "application/json");
            multipart.Add(jsonContent, "request");

            // Ajouter le fichier image
            var fileStream = File.OpenRead(imagePath);
            var fileContent = new StreamContent(fileStream);
            var mime = MimeMappingFromExtension(Path.GetExtension(imagePath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);
            multipart.Add(fileContent, "photoFile", Path.GetFileName(imagePath));

            // Envoi de la requête PUT
            var response = await httpClient.PutAsync(url, multipart);
            response.EnsureSuccessStatusCode();

            // Désérialisation de la réponse
            var responseJson = await response.Content.ReadAsStringAsync();
            var recette = JsonSerializer.Deserialize<RecetteDTO>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return recette!;
        }

        // Supprime une recette par son Id
        public async Task DeleteRecetteAsync(int idRecette)
        {
            await _rest.DeleteAsync($"{URL_DELETE_RECETTE}/{idRecette}");
        }

        #endregion Fin gestion recette

    }
}