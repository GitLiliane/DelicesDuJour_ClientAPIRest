namespace DelicesDuJour_ClientAPIRest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


            // Gestion des exceptions du thread principal (UI)
            Application.ThreadException += GlobalException.HandleThreadException;

            // Gestion des exceptions non gérées (tous threads)
            AppDomain.CurrentDomain.UnhandledException += GlobalException.HandleException;

            Application.Run(new FormBase());
        }
    }
}