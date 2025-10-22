using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) qui sert à organiser le code
// et à éviter les conflits de noms entre classes.
// Ici, on regroupe les DTO liés aux catégories.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    // Définition de la classe interne "RecetteCategorieRelationshipDTO".
    // Le mot-clé "internal" signifie que cette classe est accessible uniquement
    // au sein du même assembly (elle n'est pas visible depuis d'autres projets).
    // Ce DTO sert à représenter une relation entre une recette et une catégorie,
    // généralement utilisée pour transférer des données côté client ou API.
    internal class RecetteCategorieRelationshipDTO
    {
        // L'attribut [DisplayName] permet de définir un nom d'affichage lisible
        // pour l'interface utilisateur ou la documentation.
        // Ici, il indique "id de la catégorie" pour cette propriété.
        [DisplayName("id de la catégorie")]
        public int idCategorie { get; set; } // Identifiant de la catégorie liée à la recette.

        // Même principe pour cette propriété, l'attribut DisplayName définit un libellé
        // convivial "id de la recette" qui sera affiché dans l'interface utilisateur.
        [DisplayName("id de la recette")]
        public int idRecette { get; set; } // Identifiant de la recette liée à la catégorie.
    }
}
