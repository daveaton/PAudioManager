using AudioManager.Drivers;
using AudioManager.Helpers;
using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace AudioManager
{
    public class VolumeManager : IDisposable
    {
        #region Private Fields

        private OsVersion OS = default(OsVersion);
        private readonly List<IVolumeControl> volumeControls;
        private readonly string[] processNames;
       // private int volume;
       // private bool mute;

        #endregion Private Fields

        #region Public Properties

        public int Volume
        {
            get
            {
                return GetCurrentVolume();
            }
            set
            {
                SetCurrentVolume(value);
                //volume = value;
            }
        }

        public bool Mute
        {
            get
            {
                return GetMute();
            }
            set
            {
                SetMute(value);
                //mute = value;
            }
        }

        public bool CanListenEvents
        {
            get
            {
                return volumeControls[0].CanListen;
            }
        }

        public event VolumeChangedEventHandler VolumeChanged
        {
            add
            {
                if (CanListenEvents)
                {
                    foreach (IVolumeControl _volumeControl in volumeControls)
                    {
                        _volumeControl.VolumeChanged += value;
                        _volumeControl.StartListening();
                    }
                }
            }
            remove
            {
                if (CanListenEvents)
                {
                    foreach (IVolumeControl _volumeControl in volumeControls)
                    {
                        _volumeControl.VolumeChanged -= value;
                        _volumeControl.StopListening();
                    }
                }
            }
        }

        #endregion Public Properties

        #region Constructor

        public VolumeManager(params string[] processName)
        {
            processNames = processName;
            volumeControls = new List<IVolumeControl>();
            OS = SysInfo.Os;
            using (ServiceController process = new ServiceController("AudioSrv"))
            {
                if (process.Status != ServiceControllerStatus.Running)
                {
                    try
                    {
                        process.Start();
                        process.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 5));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            switch (OS)
            {
                case OsVersion.XP:
                    {
                        volumeControls.Add(new Drivers.XP.VolumeControl(processNames[0]));
                        break;
                    }
                case OsVersion.Vista:
                    {
                        volumeControls.Add(new Drivers.Vista.VolumeControl(processNames[0]));
                        break;
                    }
                case OsVersion.Seven:
                case OsVersion.Eight:
                    {
                        foreach (string _processName in processNames)
                        {
                            volumeControls.Add(new Drivers.Seven.VolumeControl(_processName));
                        }
                        break;
                    }
            }
            //volume = Volume;
            //mute = Mute;
        }

        #endregion Constructor

        #region Private Methods

        #region Mute

        private void SetMute(bool value)
        {
            foreach (IVolumeControl _volumeControl in volumeControls)
            {
                if (_volumeControl != null)
                {
                    _volumeControl.SetMute(value);
                }
            }
        }

        private bool GetMute()
        {
            foreach (IVolumeControl _volumeControl in volumeControls)
            {
                if (_volumeControl != null)
                {
                    return _volumeControl.GetMute();
                }
            }
            return false;
        }

        #endregion Mute

        #region Volume

        private void SetCurrentVolume(int value)
        {
            foreach (IVolumeControl _volumeControl in volumeControls)
            {
                if (_volumeControl != null)
                {
                    _volumeControl.SetVolume(value);
                }
            }
        }

        private int GetCurrentVolume()
        {
            foreach (IVolumeControl _volumeControl in volumeControls)
            {
                if (_volumeControl != null)
                {
                    return _volumeControl.GetVolume();
                }
            }
            return 0;
        }

        #endregion Volume

        #endregion Private Methods

        #region Public Methods

        public void UpdateSessions()
        {
            if (OS == OsVersion.Seven)
            {
                foreach (IVolumeControl _volumeControl in volumeControls)
                {
                    _volumeControl.UpdateSessions();
                }
            }
        }

        #endregion Public Methods

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are.
        ~VolumeManager()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                foreach (IVolumeControl volumeControl in volumeControls)
                {
                    // free managed resources here
                    if (volumeControl != null)
                    {
                        volumeControl.Dispose();
                    }
                }
            // free native resources here if there are any.
        }

        #endregion Dispose
    }
}