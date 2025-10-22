// D�claration du namespace principal de l�application cliente.
// Il regroupe les classes de d�marrage et les �l�ments globaux du projet.
namespace DelicesDuJour_ClientAPIRest
{
    // Classe "Program" contenant le point d�entr�e de l�application.
    // Elle est marqu�e comme "internal" (accessible uniquement dans l�assembly)
    // et "static" car elle ne doit pas �tre instanci�e : elle contient uniquement la m�thode Main.
    internal static class Program
    {
        /// <summary>
        /// Point d'entr�e principal de l'application.
        /// C�est ici que l�ex�cution commence lorsque l�utilisateur lance le programme.
        /// </summary>
        [STAThread] // Indique que l'application utilise un mod�le de thread STA (Single Thread Apartment),
                    // obligatoire pour les applications Windows Forms ou WPF (interface graphique).
        static void Main()
        {
            // Initialise la configuration de l'application (param�tres d�affichage, DPI, polices...).
            // Cette m�thode configure automatiquement l'environnement graphique selon les bonnes pratiques.
            // Voir la documentation : https://aka.ms/applicationconfiguration
            ApplicationConfiguration.Initialize();

            // ?? Gestion centralis�e des exceptions dans le thread principal (interface utilisateur).
            // Cela permet d�intercepter les erreurs sur le thread UI pour �viter les crashs.
            Application.ThreadException += GlobalException.HandleThreadException;

            // ?? Gestion des exceptions non g�r�es sur tous les autres threads.
            // Cela garantit qu�une erreur non pr�vue dans un thread de fond ne plante pas toute l�application.
            AppDomain.CurrentDomain.UnhandledException += GlobalException.HandleException;

            // D�marre la boucle principale de l�application avec la fen�tre de base (FormBase).
            // C�est � partir de ce formulaire que l�application se lance.
            Application.Run(new FormBase());
        }
    }
}
