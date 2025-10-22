using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) permettant de regrouper les classes DTO
// utilisées dans le domaine de l’application. Cela aide à organiser le code et à éviter
// les conflits de noms.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // Définition de la classe publique "CreateEtapeDTO".
    // Le mot-clé "public" permet à cette classe d’être accessible depuis d’autres
    // parties du projet ou d’autres projets (par exemple un contrôleur d’API).
    // Ce DTO est utilisé pour créer une nouvelle "étape" d’une recette.
    public class CreateEtapeDTO
    {
        // Propriété représentant le numéro de l’étape (ex: 1, 2, 3...).
        // Ce numéro indique l’ordre dans lequel l’étape doit être exécutée.
        public int numero { get; set; }

        // Propriété représentant le titre de l’étape.
        // Il peut s’agir d’un petit résumé ou d’un intitulé (ex: "Préparer la pâte").
        public string titre { get; set; }

        // Propriété représentant le texte descriptif de l’étape.
        // Elle contient les instructions détaillées à suivre (ex: "Mélangez la farine et le sucre...").
        public string texte { get; set; }
    }
}
