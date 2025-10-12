using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    internal class RecetteCategorieRelationshipDTO
    {
        [DisplayName("id de la catégorie")]
        public int idCategorie { get; set; }
        [DisplayName("id de la recette")]
        public int idRecette { get; set; }
    }
}
