using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) permettant d'organiser logiquement
// les classes dans le projet. Ici, il regroupe les DTO liés aux catégories.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    // Définition de la classe publique "UpdateCategorieDTO".
    // Le mot-clé "public" rend cette classe accessible depuis d'autres projets
    // ou couches de l'application (par exemple, depuis un contrôleur d'API).
    // Ce DTO est utilisé pour mettre à jour une catégorie existante.
    public class UpdateCategorieDTO
    {
        // Propriété représentant l'identifiant unique de la catégorie à modifier.
        // Cet identifiant permet de cibler la bonne catégorie lors de la mise à jour.
        public int Id { get; set; }

        // Propriété représentant le nouveau nom de la catégorie.
        // Elle contiendra la valeur mise à jour envoyée depuis le client (ex: API REST).
        public string nom { get; set; }
    }
}
