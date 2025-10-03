using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class EtapeDTO
    {
        public int id_recette { get; set; }
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
    }
}
