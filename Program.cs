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
            /*
            // single-instance logic using mutex (mutual exclusions) adapted from:
            // Eric Zhang:
            // https://github.com/Xeroday/Spotify-Ad-Blocker/blob/a985319dc6a626e16df7966c22562794c1053229/EZBlocker/EZBlocker/Program.cs
            // Sam Saffron:
            // https://stackoverflow.com/a/229567
            using (Mutex mutex = new Mutex(false,
                            $"Local\\{{{((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value}}}")
                            )
            {
                if (mutex.WaitOne(0))
                {
                    ApplicationConfiguration.Initialize();
                    Application.Run(new Context());
                }
                else
                {
                    MessageBox.Show("there is already another SpotifyAdMuter running", "SAM");
                }
            }
            */
        }
    }
}