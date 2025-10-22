using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO représentant une étape d'une recette.
    // Chaque étape décrit une partie du processus de préparation (numéro, titre et texte explicatif).
    public class EtapeDTO
    {
        // Identifiant de la recette à laquelle appartient cette étape.
        public int id_recette { get; set; }

        // Numéro de l’étape dans la recette (ordre d’exécution).
        [DisplayName("Numéro")]
        public int numero { get; set; }

        // Titre de l’étape (ex : "Préparer la pâte", "Faire cuire"...)
        [DisplayName("Titre")]
        public string titre { get; set; }

        // Description complète de ce qu’il faut faire à cette étape.
        [DisplayName("Texte")]
        public string texte { get; set; }
    }
}

