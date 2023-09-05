/*
 * App.cs
 * main logic for the SpotifyAdMuter application
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SpotifyAdMuter {
	public partial class Form : System.Windows.Forms.Form {
		private Process SpotifyProcess;
		private AppMuter SpotifyMuter;
		private string SpotifyWindowName = "spotify.exe not found";
		private bool isMuted = false;

		public Form() {
			InitializeComponent();
		}
		private void App_Load(object sender, EventArgs e) {
			Loop.Enabled = true;
		}
		private void Loop_Tick(object sender, EventArgs e) {
			if (SpotifyProcess == null) {
				MakeNewSpotifyProcess();
			}
			if (SpotifyProcess != null) {
				if (SpotifyMuter == null) {
					MakeNewSpotifyMuter();
				}
				if (SpotifyMuter != null) {
					if (SpotifyIsRunning()) {
						UpdateMuteState();
					} else {
						SpotifyProcess = null;
						SpotifyMuter = null;
					}
				}
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			if (isMuted)
				ToggleMute();
		}

		private void MakeNewSpotifyProcess() {
			SpotifyWindowName = "spotify.exe not found";
			foreach (Process process in Process.GetProcessesByName("spotify")) {
				if (process.MainWindowTitle.Length != 0) {
					SpotifyProcess = process;
					return;
				}
			}
		}
		private void MakeNewSpotifyMuter() {
			SpotifyMuter = new AppMuter(SpotifyProcess.Id);
			if (!SpotifyMuter.IsActive())
				SpotifyMuter = null;
			else
				SpotifyMuter.GetMute(out isMuted);
		}

		private void UpdateMuteState() {
			if (SpotifyHasNewTitle() && SpotifyIsPlayingAd() != isMuted)
				ToggleMute();
		}
		private bool SpotifyIsRunning() {
			if (SpotifyProcess == null)
				return false;
			SpotifyProcess.Refresh();
			return !SpotifyProcess.HasExited;
		}
		private bool SpotifyHasNewTitle() {
			if (!SpotifyWindowName.Equals(SpotifyProcess.MainWindowTitle)) {
				SpotifyWindowName = SpotifyProcess.MainWindowTitle;
//				Console.WriteLine(SpotifyWindowName);
				return true;
			}
			return false;
		}
		private bool SpotifyIsPlayingAd() {
			return !SpotifyWindowName.Contains(" - ");
		}
		private void ToggleMute() {
			if (SpotifyMuter != null)
				SpotifyMuter.SetMute(isMuted ^= true);
		}
		private void muteToggleButton_Click(object sender, EventArgs e) {
			ToggleMute();
		}
	}
}
