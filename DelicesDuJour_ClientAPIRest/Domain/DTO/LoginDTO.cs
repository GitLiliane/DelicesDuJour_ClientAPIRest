using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain.DTO
{
    // DTO utilisé pour l'authentification de l'utilisateur (envoi au serveur)
    public class LoginDTO
    {
        // Nom d'utilisateur utilisé pour se connecter
        public string Username { get; set; }

        // Mot de passe correspondant à l'utilisateur
        public string Password { get; set; }
    }
}

