using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO représentant un ingrédient d'une recette.
    // Il contient les informations essentielles sur un ingrédient : son identifiant, son nom et sa quantité.
    public class IngredientDTO
    {
        // Identifiant unique de l'ingrédient.
        public int id { get; set; }

        // Nom de l'ingrédient (ex : "Farine", "Sucre", "Beurre"...)
        [DisplayName("Nom")]
        public string nom { get; set; }

        // Quantité de l'ingrédient (ex : "200g", "3 œufs", "1 cuillère à soupe"...)
        [DisplayName("Quantité")]
        public string quantite { get; set; }
    }
}
