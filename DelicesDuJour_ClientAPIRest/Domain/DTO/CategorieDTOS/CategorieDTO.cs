using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    public class CategorieDTO
    {
        public int Id { get; set; }

        [DisplayName("Nom de la catégorie")]
        public string nom { get; set; }
    }
}
