using DelicesDuJour_ClientAPIRest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace principal de l’application.
// Cette classe fait partie de la couche de gestion globale des exceptions.
namespace DelicesDuJour_ClientAPIRest
{
    /// <summary>
    /// Classe statique responsable de la gestion centralisée des exceptions non gérées
    /// dans une application WinForms.
    /// 
    /// Elle permet d’intercepter et d’afficher proprement les erreurs au lieu de laisser
    /// l’application planter brutalement.
    /// 
    /// Ces gestionnaires doivent être initialisés dans Program.cs avant l’appel à Application.Run().
    ///     - Application.ThreadException → exceptions sur le thread principal (UI).
    ///     - AppDomain.CurrentDomain.UnhandledException → exceptions sur les threads secondaires.
    /// </summary>
    internal class GlobalException
    {
        /// <summary>
        /// Gère les exceptions non interceptées sur le thread principal (interface graphique).
        /// Ce gestionnaire est appelé lorsqu’une erreur se produit dans une action utilisateur
        /// (clic bouton, affichage, événement WinForms, etc.).
        /// </summary>
        /// <param name="sender">Objet à l’origine de l’événement.</param>
        /// <param name="e">Détails de l’exception (Exception property contient l’erreur).</param>
        public static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;

            // Affiche un message d’erreur convivial à l’utilisateur.
            // L’application ne plante pas et l’utilisateur est informé du problème.
            MessageBox.Show(GetErrorMessage(ex), "Erreur d'application", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Gère les exceptions non interceptées sur les threads d’arrière-plan (non-UI).
        /// Cela évite que des erreurs dans des tâches parallèles fassent planter toute l’application.
        /// </summary>
        /// <param name="sender">Objet à l’origine de l’événement.</param>
        /// <param name="e">Détails de l’exception encapsulée dans ExceptionObject.</param>
        public static void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            // Affiche un message d’erreur générique.
            MessageBox.Show(GetErrorMessage(ex), "Erreur d'application", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // 🔹 Méthode privée de formatage de message d’erreur.
        // Elle adapte le texte selon le type d’exception rencontré.
        private static string GetErrorMessage(Exception ex)
        {
            // Si aucune exception n’est disponible (cas très rare).
            if (ex is null)
                return "Une erreur inconnue est survenue dans l'application.";

            string message;

            // Si l’exception provient du RestClient (appel API).
            if (ex is RestClientException restEx)
            {
                // Cas où la réponse brute de l’API est disponible.
                if (restEx.HasRawContent)
                {
                    // Tente de désérialiser le contenu JSON en un objet APIError.
                    if (restEx.GetRawContent(out APIError error))
                    {
                        // Si le format correspond, affiche les détails structurés.
                        message = $"{restEx.Message}\n\n{error.Error}\n\n{error.Details}";
                    }
                    else
                    {
                        // Sinon, affiche le contenu brut reçu de l’API.
                        message = $"{restEx.Message}\n\nContenu brut de la réponse :\n\n{restEx.GetRawContent()}";
                    }
                }
                else
                {
                    // Si pas de contenu, affiche uniquement le message de l’exception.
                    message = restEx.Message;
                }
            }
            else
            {
                // Pour toute autre exception, message générique.
                message = $"Une erreur est survenue dans l'application :\n\n{ex.Message}";
            }

            return message;
        }
    }
}
