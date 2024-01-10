using System.Runtime.InteropServices;

namespace SpotifyAdMuter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Context());
            // TODO single-instance
        }
    }
}