/*
 * Program.cs
 * the main entry point of the SpotifyAdMuter application
 */

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SpotifyAdMuter {
	static class Program {
		[STAThread]
		static void Main() {
			// single-instance logic using mutex (mutual exclusions) adapted from:
			// Eric Zhang:
			// https://github.com/Xeroday/Spotify-Ad-Blocker/blob/a985319dc6a626e16df7966c22562794c1053229/EZBlocker/EZBlocker/Program.cs
			// Sam Saffron:
			// https://stackoverflow.com/a/229567
			using (Mutex mutex = new Mutex(false,
				$"Local\\{{{((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value}}}")
				) {
				if (mutex.WaitOne(0)) {
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new Form());
				} else {
					MessageBox.Show("there is already another SpotifyAdMuter running", "SAM");
				}
			}
		}
	}
}
