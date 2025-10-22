using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO utilisé pour la mise à jour d'une recette existante
    public class UpdateRecetteDTO
    {
        // Identifiant unique de la recette à modifier
        public int Id { get; set; }

        // Nom de la recette (affiché dans les formulaires avec ce libellé)
        [DisplayName("Nom de la recette")]
        public string nom { get; set; }

        // Temps de préparation de la recette (de type TimeSpan pour représenter une durée)
        [DataType(DataType.Time)]
        [DisplayName("Temps de préparation")]
        public TimeSpan temps_preparation { get; set; }

        // Temps de cuisson (également représenté en TimeSpan)
        [DataType(DataType.Time)]
        [DisplayName("Temps de cuisson")]
        public TimeSpan temps_cuisson { get; set; }

        // Niveau de difficulté (entre 1 et 3)
        [Range(1, 3)]
        [DisplayName("Difficulté")]
        public int difficulte { get; set; }

        // Liste des étapes associées à la recette
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();

        // Liste des ingrédients nécessaires pour la recette
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        // Liste des catégories auxquelles appartient la recette
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        // Chemin d'accès à la photo sur le serveur (stockée dans la base ou via l'API)
        public string? photo { get; set; }

        // Chemin local de l'image sur le poste client (non transmis à l'API)
        public string? ImagePathLocal { get; set; }
    }
}
