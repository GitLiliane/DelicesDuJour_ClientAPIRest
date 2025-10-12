using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class EtapeDTO
    {
        public int id_recette { get; set; }

        [DisplayName("Numéro")]
        public int numero { get; set; }

        [DisplayName("Titre")]
        public string titre { get; set; }

        [DisplayName("Texte")]
        public string texte { get; set; }
    }
}
