using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest.Domain
{
    // Classe représentant une erreur renvoyée par l'API
    public class APIError
    {
        // Message d'erreur principal retourné par l'API
        public string Error { get; set; }

        // Détails supplémentaires sur l'erreur (ex : stacktrace, description technique)
        public string Details { get; set; }
    }
}

