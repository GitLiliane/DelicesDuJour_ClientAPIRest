using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) permettant d’organiser les classes
// de manière logique dans le projet. Ici, il regroupe les DTO relatifs aux catégories.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    // Définition d'une classe interne (internal), ce qui signifie qu'elle n'est accessible
    // qu'à l'intérieur du même assembly (projet). Elle ne peut pas être utilisée directement
    // depuis un autre projet.
    // Ce DTO semble servir à représenter la relation entre une recette et une catégorie,
    // par exemple pour associer une recette à une catégorie dans la base de données.
    internal class CreateRecetteCategorieRelationshipDTO
    {
        // Propriété représentant l'identifiant de la catégorie concernée.
        // C'est un entier (int) qui fait probablement référence à une catégorie existante.
        public int idCategorie { get; set; }

        // Propriété représentant l'identifiant de la recette concernée.
        // C'est également un entier, servant à identifier la recette à relier à la catégorie.
        public int idRecette { get; set; }
    }
}
