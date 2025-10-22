using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) qui permet d'organiser les classes
// dans le projet. Ici, il regroupe les DTO liés au domaine de l'application.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // Définition de la classe publique "CreateIngredientDTO".
    // Le mot-clé "public" rend cette classe accessible depuis d'autres projets
    // ou couches de l'application (par exemple, depuis un contrôleur d'API).
    // Ce DTO est utilisé pour créer un nouvel ingrédient associé à une recette.
    public class CreateIngredientDTO
    {
        // Propriété représentant le nom de l'ingrédient (ex: "Sucre", "Farine").
        // Elle correspond au libellé qui sera affiché ou enregistré dans la base de données.
        public string nom { get; set; }

        // Propriété représentant la quantité de l'ingrédient (ex: "100g", "2 cuillères").
        // C'est une chaîne de caractères pour permettre une notation flexible des unités.
        public string quantite { get; set; }
    }
}
