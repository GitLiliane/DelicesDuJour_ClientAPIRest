using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    public class UpdateRecetteDTO
    {
        public int Id { get; set; }

        [DisplayName("Nom de la recette")]
        public string nom { get; set; }

        [DataType(DataType.Time)]
        [DisplayName("Temps de préparation")]
        public TimeSpan temps_preparation { get; set; }


        [DataType(DataType.Time)]
        [DisplayName("Temps de cuisson")]
        public TimeSpan temps_cuisson { get; set; }

        [Range(1, 3)]
        [DisplayName("Difficulté")]
        public int difficulte { get; set; }
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        public string? photo { get; set; } // chemin de stockage sur le serveur
        public string? ImagePathLocal { get; set; } // chemin local sur ton PC (non envoyé dans le JSON)
    }
}
