using System.Diagnostics;
using SpotifyAdMuter.Properties;

namespace SpotifyAdMuter
{
    public class Context : ApplicationContext
    {
        private Process? _spotifyProcess;
        private AppMuter? _spotifyMuter;
        private string _spotifyWindowName = string.Empty;
        private bool _isMuted = false;
        private readonly NotifyIcon _notifyIcon;
        private readonly System.Windows.Forms.Timer _timer;

        private Process? SpotifyProcess
        {
            get => _spotifyProcess;
            set
            {
                _spotifyProcess = value;
                Debug.WriteLine($"SpotifyProcess.Id = {value?.Id}");
            }
        }

        private string SpotifyWindowName
        {
            get => _spotifyWindowName;
            set
            {
                _spotifyWindowName = value;
                Debug.WriteLine($"SpotifyWindowName = {value}");

            }
        }

        public Context()
        {
            _notifyIcon = new NotifyIcon()
            {
                Text = "unmuted",
                Icon = Resources.UnmutedIcon,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items = { new ToolStripMenuItem("Exit", null, Exit) }
                },
                Visible = true
            };
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _timer = new System.Windows.Forms.Timer()
            {
                Interval = 500, // milliseconds
            };
            _timer.Tick += Timer_Tick;
            _timer.Enabled = true;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (SpotifyProcess == null)
            {
                MakeNewSpotifyProcess();
                if (SpotifyProcess == null) return;
            }
            if (_spotifyMuter == null)
            {
                MakeNewSpotifyMuter();
                if (_spotifyMuter == null) return;
            }
            if (SpotifyIsRunning())
            {
                UpdateMuteState();
            }
            else
            {
                SpotifyProcess = null;
                _spotifyMuter = null;
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
            _spotifyMuter = new AppMuter(SpotifyProcess.Id);
            if (!_spotifyMuter.IsActive())
                _spotifyMuter = null;
            else
                _spotifyMuter.GetMute(out _isMuted);
        }

        private void UpdateMuteState()
        {
            if (SpotifyHasNewTitle() && SpotifyIsPlayingAd() != _isMuted)
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
            if (_spotifyMuter != null)
            {
                _spotifyMuter.SetMute(_isMuted ^= true);
                if (_isMuted)
                {
                    _notifyIcon.Text = $"muted \"{SpotifyWindowName}\"";
                    _notifyIcon.Icon = Resources.MutedIcon;
                }
                else
                {
                    _notifyIcon.Text = $"unmuted \"{SpotifyWindowName}\"";
                    _notifyIcon.Icon = Resources.UnmutedIcon;
                }
            }
        }
        void Exit(object? sender, EventArgs e)
        {
            if (_spotifyMuter != null) _spotifyMuter.SetMute(false);
            Application.Exit();
        }
    }
}
