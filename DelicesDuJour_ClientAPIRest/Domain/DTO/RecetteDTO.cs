using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTOS
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
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();
        //public List<int>? categorie_ids { get; set; } = new List<int>();
        //public string? photo { get; set; }
        //public IFormFile? photoFile { get; set; }

    }
}
