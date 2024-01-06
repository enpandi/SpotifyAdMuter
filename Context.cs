using System.Diagnostics;
using SpotifyAdMuter.Properties;

namespace SpotifyAdMuter
{
    public class Context : ApplicationContext
    {
        private Process? _SpotifyProcess;
        private AppMuter? SpotifyMuter;
        private string _SpotifyWindowName = string.Empty;
        private bool isMuted = false;
        private NotifyIcon notifyIcon;
        private readonly System.Windows.Forms.Timer timer;

        private Process? SpotifyProcess
        {
            get => _SpotifyProcess;
            set
            {
                _SpotifyProcess = value;
                Debug.WriteLine($"SpotifyProcess.Id = {value?.Id}");
            }
        }

        private string SpotifyWindowName
        {
            get => _SpotifyWindowName;
            set
            {
                _SpotifyWindowName = value;
                Debug.WriteLine($"SpotifyWindowName = {value}");

            }
        }

        public Context()
        {
            notifyIcon = new NotifyIcon()
            {
                Text = "unmuted",
                Icon = Resources.UnmutedIcon,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items = { new ToolStripMenuItem("Exit", null, Exit) }
                },
                Visible = true
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            timer = new System.Windows.Forms.Timer()
            {
                Interval = 500, // milliseconds
            };
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (SpotifyProcess == null)
            {
                MakeNewSpotifyProcess();
                if (SpotifyProcess == null) return;
            }
            if (SpotifyMuter == null)
            {
                MakeNewSpotifyMuter();
                if (SpotifyMuter == null) return;
            }
            if (SpotifyIsRunning())
            {
                UpdateMuteState();
            }
            else
            {
                SpotifyProcess = null;
                SpotifyMuter = null;
            }
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ToggleMute();
        }

        private void MakeNewSpotifyProcess()
        {
            SpotifyWindowName = string.Empty;
            foreach (Process process in Process.GetProcessesByName("spotify"))
            {
                if (process.MainWindowTitle.Length != 0)
                {
                    SpotifyProcess = process;
                    return;
                }
            }
        }
        private void MakeNewSpotifyMuter()
        {
            Debug.Assert(SpotifyProcess != null);
            SpotifyMuter = new AppMuter(SpotifyProcess.Id);
            if (!SpotifyMuter.IsActive())
                SpotifyMuter = null;
            else
                SpotifyMuter.GetMute(out isMuted);
        }

        private void UpdateMuteState()
        {
            if (SpotifyHasNewTitle() && SpotifyIsPlayingAd() != isMuted)
                ToggleMute();
        }
        private bool SpotifyIsRunning()
        {
            if (SpotifyProcess == null)
                return false;
            SpotifyProcess.Refresh();
            return !SpotifyProcess.HasExited;
        }
        private bool SpotifyHasNewTitle()
        {
            Debug.Assert(SpotifyProcess != null);
            if (!SpotifyWindowName.Equals(SpotifyProcess.MainWindowTitle))
            {
                SpotifyWindowName = SpotifyProcess.MainWindowTitle;
                return true;
            }
            return false;
        }
        private bool SpotifyIsPlayingAd()
        {
            return !SpotifyWindowName.Contains(" - ");
        }
        private void ToggleMute()
        {
            if (SpotifyMuter != null)
            {
                SpotifyMuter.SetMute(isMuted ^= true);
                if (isMuted)
                {
                    notifyIcon.Text = $"muted \"{SpotifyWindowName}\"";
                    notifyIcon.Icon = Resources.MutedIcon;
                }
                else
                {
                    notifyIcon.Text = $"unmuted \"{SpotifyWindowName}\"";
                    notifyIcon.Icon = Resources.UnmutedIcon;
                }
            }
        }
        void Exit(object? sender, EventArgs e)
        {
            if (isMuted) ToggleMute();
            Application.Exit();
        }
    }
}
