using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    internal class RecetteDTO
    {
        public int Id { get; set; }
        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }


        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        [Range(1, 3)]
        public int difficulte { get; set; }
        //public List<Etape>? etapes { get; set; } = new List<Etape>();
        //public List<Ingredient>? ingredients { get; set; } = new List<Ingredient>();

        //public List<Categorie> categories { get; set; } = new List<Categorie>();
        public List<int>? categorie_ids { get; set; } = new List<int>();
        public string? photo { get; set; }
        //public IFormFile? photoFile { get; set; }

    }
}
