using DelicesDuJour_ClientAPIRest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest
{

    /// <summary>
    /// Classe statique pour centraliser la gestion des exceptions non gérées dans une application WinForms.
    /// Le branchement de ces gestionnaires d'exceptions doit être effectué dans Program.cs avant Application.Run().
    ///     - Application.ThreadException pour les exceptions dans le thread principal (UI).
    ///     - AppDomain.CurrentDomain.UnhandledException pour les exceptions dans les threads en arrière-plan (background threads).
    /// </summary>
    internal class GlobalException
    {
        /// <summary>
        /// Méthode appelée lorsqu'une exception non gérée survient dans le thread principal de l'interface utilisateur (UI).
        /// </summary>
        /// <param name="sender">L'objet source de l'événement.</param>
        /// <param name="e">Les arguments contenant l'exception.</param>
        public static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;

            // Affiche du message d'erreur
            MessageBox.Show(GetErrorMessage(ex), "Erreur d'application", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Méthode appelée lorsqu'une exception non gérée survient dans un thread autre que le thread principal (background thread).
        /// </summary>
        /// <param name="sender">L'objet source de l'événement.</param>
        /// <param name="e">Les arguments contenant l'exception.</param>
        public static void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            // Affiche du message d'erreur
            MessageBox.Show(GetErrorMessage(ex), "Erreur d'application", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string GetErrorMessage(Exception ex)
        {
            if (ex is null)
                return "Une erreur inconnue est survenue dans l'application.";

            string message;

            if (ex is RestClientException restEx)
            {
                if (restEx.HasRawContent)
                {
                    if (restEx.GetRawContent(out APIError error))
                    {
                        message = $"{restEx.Message}\n\n{error.Error}\n\n{error.Details}";
                    }
                    else
                    {
                        message = $"{restEx.Message}\n\nContenu brut de la réponse :\n\n{restEx.GetRawContent()}";
                    }
                }
                else
                {
                    message = restEx.Message;
                }
            }
            else
            {
                message = $"Une erreur est survenue dans l'application :\n\n{ex.Message}";
            }

            return message;
        }
    }
}
