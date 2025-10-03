using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class CreateEtapeDTO
    {      
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
    }
}
