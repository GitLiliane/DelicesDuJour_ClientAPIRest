using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO utilisé lors de la création d’une nouvelle recette.
    // Sert à transmettre les informations nécessaires à l’API pour enregistrer une recette.
    public class CreateRecetteDTO
    {
        // Nom de la recette.
        public string nom { get; set; }

        // Durée de préparation de la recette.
        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }

        // Durée de cuisson de la recette.
        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        // Niveau de difficulté de la recette (entre 1 et 3).
        [Range(1, 3)]
        public int difficulte { get; set; }

        // Liste des étapes de préparation associées à la recette.
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();

        // Liste des ingrédients nécessaires pour la recette.
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        // Liste des catégories auxquelles appartient la recette (ex : Dessert, Plat, etc.).
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        // Chemin de stockage de l’image sur le serveur.
        public string? photo { get; set; }

        // Chemin local de l’image sur le poste client (non envoyé dans le JSON).
        public string? ImagePathLocal { get; set; }
    }
}
