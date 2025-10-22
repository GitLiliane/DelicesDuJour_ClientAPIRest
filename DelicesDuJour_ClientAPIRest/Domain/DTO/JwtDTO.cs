using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO servant à représenter la réponse du serveur lors d'une authentification réussie.
    // Il contient le jeton JWT (JSON Web Token) renvoyé par l'API.
    internal class JwtDTO
    {
        // Le jeton JWT fourni par le serveur après connexion,
        // utilisé pour authentifier les requêtes suivantes.
        public string Token { get; set; }
    }
}
