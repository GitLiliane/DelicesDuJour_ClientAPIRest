using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTOS
{
    // DTO représentant une recette complète telle qu’elle est renvoyée ou reçue depuis l’API
    internal class RecetteDTO
    {
        // Identifiant unique de la recette
        public int Id { get; set; }

        // Nom de la recette (affiché sous ce libellé dans les formulaires ou grilles)
        [DisplayName("Nom de la recette")]
        public string nom { get; set; }

        // Temps de préparation de la recette
        [DataType(DataType.Time)]
        [DisplayName("Temps de préparation")]
        public TimeSpan temps_preparation { get; set; }

        // Temps de cuisson de la recette
        [DataType(DataType.Time)]
        [DisplayName("Temps de cuisson")]
        public TimeSpan temps_cuisson { get; set; }

        // Niveau de difficulté compris entre 1 et 3
        [Range(1, 3)]
        [DisplayName("Difficulté")]
        public int difficulte { get; set; }

        // Liste des étapes de la recette (ex: préparation, cuisson, dressage…)
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();

        // Liste des ingrédients nécessaires à la recette
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        // Liste des catégories associées à la recette (ex: dessert, entrée, plat…)
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        // Nom ou chemin de la photo de la recette (stocké côté serveur)
        public string? photo { get; set; }

        // Propriété optionnelle pour l’envoi de fichier (commentée ici, non utilisée côté client)
        // public IFormFile? photoFile { get; set; }
    }
}
