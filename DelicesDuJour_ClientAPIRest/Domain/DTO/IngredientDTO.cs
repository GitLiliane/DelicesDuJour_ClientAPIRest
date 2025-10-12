using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class IngredientDTO
    {
        public int id { get; set; }

        [DisplayName("Nom")]
        public string nom { get; set; }

        [DisplayName("Quantité")]
        public string quantite { get; set; }
    }
}
