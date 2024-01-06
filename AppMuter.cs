/*
 * AppMuter.cs
 * utility class for muting/unmuting Windows processes
 * uses the Windows Core Audio API
 */

using System.Runtime.InteropServices;

namespace SpotifyAdMuter
{
    class AppMuter
    {
        private ISimpleAudioVolume _volumeController;

        public bool IsActive() { return _volumeController != null; }

        public void SetMute(bool mute) { _volumeController.SetMute(mute, Guid.Empty); }

        public void GetMute(out bool mute) { mute = false; _volumeController.GetMute(out mute); }

        public AppMuter(int pid)
        {
            // logic for constructing an ISimpleAudioVolume adapted from:
            // Eric Zhang:
            // lines 69-125 of https://github.com/Xeroday/Spotify-Ad-Blocker/blob/a985319dc6a626e16df7966c22562794c1053229/EZBlocker/EZBlocker/AudioUtils.cs#L69
            // Simon Mourier:
            // https://stackoverflow.com/a/14322736
            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice speakers = null;
            IAudioSessionManager2 sessionManager = null;
            IAudioSessionEnumerator sessionEnumerator = null;
            int ctlPid = 0;
            try
            {
                // get default device
                deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

                // get session manager
                Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
                speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
                sessionManager = (IAudioSessionManager2)o;

                // get sessions
                sessionManager.GetSessionEnumerator(out sessionEnumerator);
                sessionEnumerator.GetCount(out int numSessions);

                // get volume control
                for (int i = 0; i < numSessions; i++)
                {
                    IAudioSessionControl2 ctl = null;
                    try
                    {
                        sessionEnumerator.GetSession(i, out ctl);
                        if (ctl == null)
                            continue;

                        // get and compare process id
                        ctl.GetProcessId(out ctlPid);
                        if (pid == ctlPid)
                        {
                            _volumeController = ctl as ISimpleAudioVolume;
                            break;
                        }
                    }
                    finally
                    {
                        if (_volumeController == null && ctl != null)
                            Marshal.ReleaseComObject(ctl);
                    }
                }
            }
            finally
            {
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (sessionManager != null) Marshal.ReleaseComObject(sessionManager);
                if (speakers != null) Marshal.ReleaseComObject(speakers);
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
            }
        }

        // Windows Core Audio imports adapted from:
        // Eric Zhang:
        // lines 140-259 of https://github.com/Xeroday/Spotify-Ad-Blocker/blob/a985319dc6a626e16df7966c22562794c1053229/EZBlocker/EZBlocker/AudioUtils.cs#L140
        // Simon Mourier:
        // https://stackoverflow.com/a/14322736
        #region Windows Core Audio imports
        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISimpleAudioVolume
        {
            [PreserveSig] int SetMasterVolume(float fLevel, Guid EventContext);
            [PreserveSig] int GetMasterVolume(out float pfLevel);
            [PreserveSig] int SetMute(bool bMute, Guid EventContext);
            [PreserveSig] int GetMute(out bool pbMute);
        }

        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"), ComImport]
        private class MMDeviceEnumerator { }

        private enum EDataFlow { eRender, eCapture, eAll }

        private enum ERole { eConsole, eMultimedia, eCommunications }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            int NotImpl1();
            [PreserveSig] int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppEndpoint);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig] int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionManager2
        {
            int NotImpl1();
            int NotImpl2();
            [PreserveSig] int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionEnumerator
        {
            [PreserveSig] int GetCount(out int SessionCount);
            [PreserveSig] int GetSession(int SessionCount, out IAudioSessionControl2 Session);
        }

        [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionControl2
        {
            // IAudioSessionControl
            [PreserveSig] int NotImpl0();
            [PreserveSig] int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig] int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            [PreserveSig] int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig] int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            [PreserveSig] int GetGroupingParam(out Guid pRetVal);
            [PreserveSig] int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            [PreserveSig] int NotImpl1();
            [PreserveSig] int NotImpl2();
            // IAudioSessionControl2
            [PreserveSig] int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig] int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            [PreserveSig] int GetProcessId(out int pRetVal);
            [PreserveSig] int IsSystemSoundsSession();
            [PreserveSig] int SetDuckingPreference(bool optOut);
        }
        #endregion
    }
}
