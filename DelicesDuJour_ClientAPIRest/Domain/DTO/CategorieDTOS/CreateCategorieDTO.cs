using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) qui sert à organiser le code
// dans une structure hiérarchique et éviter les conflits de noms.
// Ici, il s'agit du dossier des DTO liés aux catégories.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    // Définition d'une classe "CreateCategorieDTO"
    // Le mot-clé "internal" signifie que cette classe est accessible uniquement
    // à l'intérieur du même assembly (elle n'est pas visible depuis d'autres projets).
    // Ce DTO est probablement utilisé pour la création d'une nouvelle catégorie.
    internal class CreateCategorieDTO
    {
        // Propriété "nom" de type string, qui représente le nom de la catégorie à créer.
        // Elle sera envoyée (par exemple via une requête HTTP POST) pour créer une nouvelle entrée.
        public string nom { get; set; }
    }
}
