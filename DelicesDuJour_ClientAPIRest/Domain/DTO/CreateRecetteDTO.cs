using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class CreateRecetteDTO
    {        
        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }


        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        [Range(1, 3)]
        public int difficulte { get; set; }
        public List<CreateEtapeDTO>? etapes { get; set; } = new List<CreateEtapeDTO>();
        public List<CreateIngredientDTO>? ingredients { get; set; } = new List<CreateIngredientDTO>();

        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();
        //public List<int>? categorie_ids { get; set; } = new List<int>();
        //public string? photo { get; set; }
        //public IFormFile? photoFile { get; set; }
    }
}
