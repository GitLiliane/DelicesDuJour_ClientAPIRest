// Déclaration du namespace principal de l’application cliente.
// Il regroupe les classes de démarrage et les éléments globaux du projet.
namespace DelicesDuJour_ClientAPIRest
{
    // Classe "Program" contenant le point d’entrée de l’application.
    // Elle est marquée comme "internal" (accessible uniquement dans l’assembly)
    // et "static" car elle ne doit pas être instanciée : elle contient uniquement la méthode Main.
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// C’est ici que l’exécution commence lorsque l’utilisateur lance le programme.
        /// </summary>
        [STAThread] // Indique que l'application utilise un modèle de thread STA (Single Thread Apartment),
                    // obligatoire pour les applications Windows Forms ou WPF (interface graphique).
        static void Main()
        {
            // Initialise la configuration de l'application (paramètres d’affichage, DPI, polices...).
            // Cette méthode configure automatiquement l'environnement graphique selon les bonnes pratiques.
            // Voir la documentation : https://aka.ms/applicationconfiguration
            ApplicationConfiguration.Initialize();

            // ?? Gestion centralisée des exceptions dans le thread principal (interface utilisateur).
            // Cela permet d’intercepter les erreurs sur le thread UI pour éviter les crashs.
            Application.ThreadException += GlobalException.HandleThreadException;

            // ?? Gestion des exceptions non gérées sur tous les autres threads.
            // Cela garantit qu’une erreur non prévue dans un thread de fond ne plante pas toute l’application.
            AppDomain.CurrentDomain.UnhandledException += GlobalException.HandleException;

            // Démarre la boucle principale de l’application avec la fenêtre de base (FormBase).
            // C’est à partir de ce formulaire que l’application se lance.
            Application.Run(new FormBase());
        }
    }
}
