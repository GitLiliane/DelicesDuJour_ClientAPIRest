using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTO
{
    internal class RecetteCategorieRelationshipDTO
    {
        public int idCategorie { get; set; }
        public int idRecette { get; set; }
    }
}
