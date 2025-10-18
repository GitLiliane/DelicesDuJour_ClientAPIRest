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
        const string URL_POST_LOGIN = "/api/Authentication/Login/";

        const string URL_GET_RECETTES = "api/Recettes";
        const string URL_GET_RECETTE_BY_ID = "api/Recettes";
        const string URL_GET_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations";
        const string URL_POST_RECETTE = "api/Recettes";
        const string URL_PUT_RECETTE = "api/Recettes";
        const string URL_DELETE_RECETTE = "api/Recettes";

        const string URL_GET_CATEGORIES = "api/Categories";
        const string URL_POST_CATEGORIES = "api/Categories";
        const string URL_PUT_CATEGORIES = "api/Categories";
        const string URL_DELETE_CATEGORIES = "api/Categories";

        const string URL_GET_RECETTES_BY_IDCATEGORIE = "api/RecettesCategoriesRelations/GetRecettesByIdCategorie";
        const string URL_GET_CATEGORIES_BY_RECETTE = "api/RecettesCategoriesRelations/GetCategoriesByIdRecette";
        const string URL_POST_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations/AddRecetteCategorieRelationship";
        const string URL_REMOVE_RECETTES_CATEGORIES_RELATIONS = "api/RecettesCategoriesRelations/RemoveRecetteCategorieRelationship";

        private readonly RestClient _rest = RestClient.Instance;

        #region Singleton
        private static readonly object _lock = new object();
        private static DeliceService _instance;
        public static DeliceService Instance
        {
            get
            {
                if (_instance is null)
                {
                    //plusieurs thread
                    lock (_lock) //multithreading
                    {
                        //1 seul thread à la fois
                        if (_instance is null)
                        {
                            _instance = new DeliceService();
                        }
                    }
                }

                return _instance;
            }
        }
        #endregion
        #region Login

        public async Task<bool> Login(string baseurl, LoginDTO loginDTO)
        {
            _rest.BaseUrl = baseurl;
            var res = await _rest.PostAsync<JwtDTO, LoginDTO>(URL_POST_LOGIN, loginDTO);
            if (res != null)
            {
                _rest.JwtToken = res.Token;
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<string> GetRolesFromJwt(string[] possibleClaimTypes = null)
        {
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

                var roles = jwtToken.Claims
                    .Where(c => possibleClaimTypes.Contains(c.Type))
                    .Select(c => c.Value);

                return roles;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors du décodage du JWT : {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            return !string.IsNullOrEmpty(_rest.JwtToken);
        }

        public void Logout()
        {
            _rest.JwtToken = null;
        }

        #endregion Fin Login

        #region Recette

        public async Task<IEnumerable<RecetteDTO>> GetRecettesAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteDTO>>($"{URL_GET_RECETTES}");
            return res;
        }

        public async Task<RecetteDTO> GetRecetteByIdAsync(int idRecette)
        {
            try
            {

                var res = await _rest.GetAsync<RecetteDTO>($"{URL_GET_RECETTE_BY_ID}/{idRecette}");
                return res;
            }
            catch (Exception e)
            {
                throw new Exception("Impossible de récupérer la recette");
            }

        }
        public async Task<IEnumerable<RecetteDTO>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteDTO>>(
                    $"{URL_GET_RECETTES_BY_IDCATEGORIE}/{idCategorie}");

            return res;
        }

        #endregion Fin Recette

        #region Catégories

        public async Task<IEnumerable<CategorieDTO>> GetCategoriesAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<CategorieDTO>>($"{URL_GET_CATEGORIES}");
            return res;
        }

        public async Task<CategorieDTO> AddCategorieAsync(CreateCategorieDTO createDTO)
        {
            var res = await _rest.PostAsync<CategorieDTO, CreateCategorieDTO>($"{URL_POST_CATEGORIES}", createDTO);
            return res;
        }

        public async Task<CategorieDTO> UpdateCategorieAsync(UpdateCategorieDTO updateDTO, int id)
        {
            var res = await _rest.PutAsync<CategorieDTO, UpdateCategorieDTO>($"{URL_PUT_CATEGORIES}/{id}", updateDTO);
            return res;
        }

        public async Task DeleteCategorieAsync(int id)
        {
            await _rest.DeleteAsync($"{URL_DELETE_CATEGORIES}/{id}");
        }

        #endregion Fin Catégories

        #region Relation Recettes Catégories

        public async Task<IEnumerable<CategorieDTO>> GetCategoriesByIdRecette(int id)
        {
            var res = await _rest.GetAsync<IEnumerable<CategorieDTO>>(
                    $"{URL_GET_CATEGORIES_BY_RECETTE}/{id}");
            return res;
        }

        public async Task AddRelationRecetteCategorieAsync(int idCategorie, int idRecette)
        {
            await _rest.PostAsync($"{URL_POST_RECETTES_CATEGORIES_RELATIONS}/{idCategorie}/{idRecette}");
        }

        public async Task DeleteRelationRecetteCategorieAsync(int idCategorie, int idRecette)
        {
            await _rest.DeleteAsync($"{URL_REMOVE_RECETTES_CATEGORIES_RELATIONS}/{idCategorie}/{idRecette}");
        }

        public async Task<IEnumerable<RecetteCategorieRelationshipDTO>> GetRecetteCategorieRelationshipsAsync()
        {
            var res = await _rest.GetAsync<IEnumerable<RecetteCategorieRelationshipDTO>>(URL_GET_RECETTES_CATEGORIES_RELATIONS);
            return res;
        }

        #endregion Fin Relation Recettes Catégories

        #region Gestion Recette
      
        public async Task<RecetteDTO> CreateRecetteAsync(CreateRecetteDTO dto, string imagePath = null)
        {
            // Cas sans image : PUT simple
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                var res = await _rest.PostAsync<RecetteDTO, CreateRecetteDTO>(URL_POST_RECETTE, dto);
                return res;
            }

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

            // Le nom "request" doit correspondre au paramètre côté API
            multipart.Add(jsonContent, "request");

            // Ajouter le fichier image 

            var fileStream = File.OpenRead(imagePath);
            var fileContent = new StreamContent(fileStream);
            var mime = MimeMappingFromExtension(Path.GetExtension(imagePath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);

            // Le nom "photoFile" doit correspondre au paramètre côté API
            multipart.Add(fileContent, "photoFile", Path.GetFileName(imagePath));


            // Envoyer la requête
            var response = await httpClient.PostAsync(url, multipart);

            // Lancer une exception si l'API retourne une erreur
            response.EnsureSuccessStatusCode();

            // Lire et désérialiser la réponse en RecetteDTO
            var responseJson = await response.Content.ReadAsStringAsync();
            var recette = JsonSerializer.Deserialize<RecetteDTO>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return recette!;
        }



        // Helper simple pour déduire le MIME type depuis l'extension (très minimal)
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

            // Cas avec image
            if (string.IsNullOrWhiteSpace(RestClient.Instance.JwtToken))
                throw new InvalidOperationException("Le token JWT n'est pas défini. Connectez-vous d'abord.");

            // ⚠️ Inclure l'ID dans l'URL pour que le controller le reçoive
            string url = $"{RestClient.Instance.BaseUrl}/api/recettes/{updateRecette.Id}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", RestClient.Instance.JwtToken);

            using var multipart = new MultipartFormDataContent();

            // Sérialiser le DTO en JSON
            var dtoJson = JsonSerializer.Serialize(updateRecette, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var jsonContent = new StringContent(dtoJson, Encoding.UTF8, "application/json");
            multipart.Add(jsonContent, "request");

            // Ajouter le fichier image
            var fileStream = File.OpenRead(imagePath);
            var fileContent = new StreamContent(fileStream);
            var mime = MimeMappingFromExtension(Path.GetExtension(imagePath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);
            multipart.Add(fileContent, "photoFile", Path.GetFileName(imagePath));

            // Envoyer la requête avec l'ID dans l'URL
            var response = await httpClient.PutAsync(url, multipart);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var recette = JsonSerializer.Deserialize<RecetteDTO>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return recette!;
        }


        public async Task DeleteRecetteAsync(int idRecette)
        {
            await _rest.DeleteAsync($"{URL_DELETE_RECETTE}/{idRecette}");
        }

        #endregion Fin gestion recette
    }
}