using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration du namespace (espace de noms) qui organise le code dans une structure logique.
// Ici, la classe fait partie du domaine "DTO" (Data Transfer Object) lié aux catégories.
namespace DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS
{
    // Définition de la classe CategorieDTO
    // Un DTO (Data Transfer Object) sert à transporter des données entre différentes couches
    // (par exemple entre le serveur et le client) sans logique métier.
    public class CategorieDTO
    {
        // Propriété représentant l'identifiant unique de la catégorie.
        // Le type "int" indique qu'il s'agit d'un nombre entier.
        public int Id { get; set; }

        // L'attribut [DisplayName] vient du namespace System.ComponentModel.
        // Il permet de spécifier un nom d’affichage plus lisible dans les interfaces utilisateurs,
        // comme dans une application web ou un outil d'administration.
        [DisplayName("Nom de la catégorie")]
        public string nom { get; set; }  // Propriété qui contient le nom de la catégorie.
    }
}

