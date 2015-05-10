using AudioManager.Helpers;
using CoreAudioApi;
using CoreAudioApi.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioManager.Drivers.Seven
{
    public class VolumeControl : IVolumeControl
    {
        #region Private Fields

        private readonly string _processName;
        private bool isListening/* = false*/;
        private SessionHandler handler;
        private float _volume;
        private bool _mute;

        #endregion Private Fields

        #region Constructor

        public VolumeControl(string processName)
        {
            _processName = processName;
            handler = new SessionHandler();
            handler.Handler.VolumeChanged += OnVolumeChanged;
        }

        #endregion Constructor

        #region Interface

        #region CanListen

        protected virtual bool CanListen
        {
            get
            {
                return ((IVolumeControl)this).CanListen;
            }
        }

        bool IVolumeControl.CanListen
        {
            get { return true; }
        }

        #endregion CanListen

        #region Set Volume

        protected virtual void SetVolume(int value)
        {
            ((IVolumeControl)this).SetVolume(value);
        }

        void IVolumeControl.SetVolume(int value)
        {
            float newVolume = ((float)value / 100);
            if (newVolume > 1)
                newVolume = 1;
            if (newVolume < 0)
                newVolume = 0;
            _volume = newVolume;
            List<AudioSession> sessions = GetSessions();
            if (sessions.Count > 0)
            {
                foreach (AudioSession session in sessions)
                {
                    SimpleAudioVolume volume = session.Session.SimpleAudioVolume;
                    volume.MasterVolume = newVolume;
                }
            }
        }

        #endregion Set Volume

        #region Set Mute

        protected virtual void SetMute(bool value)
        {
            ((IVolumeControl)this).SetMute(value);
        }

        void IVolumeControl.SetMute(bool value)
        {
            _mute = value;
            List<AudioSession> controls = GetSessions();
            foreach (AudioSession session in controls)
            {
                SimpleAudioVolume volume = session.Session.SimpleAudioVolume;
                volume.Mute = value;
            }
        }

        #endregion Set Mute

        #region Get Volume

        protected virtual int GetVolume()
        {
            return ((IVolumeControl)this).GetVolume();
        }

        int IVolumeControl.GetVolume()
        {
            float result = 0;

            List<AudioSession> sessions = GetSessions();
            if (sessions.Count > 0)
            {
                foreach (AudioSession session in sessions)
                {
                    SimpleAudioVolume volume = session.Session.SimpleAudioVolume;
                    result += volume.MasterVolume;
                }
                result = result / sessions.Count;
                _volume = result;
                return (int)(result * 100);
            }
            else
            {
                _volume = 0;
                return 0;
            }
        }

        #endregion Get Volume

        #region Get Mute

        protected virtual bool GetMute()
        {
            return ((IVolumeControl)this).GetMute();
        }

        bool IVolumeControl.GetMute()
        {
            bool result = false;

            List<AudioSession> sessions = GetSessions();
            if (sessions.Count > 0)
            {
                foreach (AudioSession session in sessions)
                {
                    SimpleAudioVolume volume = session.Session.SimpleAudioVolume;
                    result = result || volume.Mute;
                }
                _mute = result;
                return result;
            }
            _mute = result;
            return result;
        }

        #endregion Get Mute

        #region StartListening

        protected virtual void StartListening()
        {
            ((IVolumeControl)this).StartListening();
        }

        void IVolumeControl.StartListening()
        {
            if (!isListening)
            {
                isListening = true;
                List<AudioSession> sessions = GetSessions();
                if (sessions.Count > 0)
                {
                    foreach (AudioSession session in sessions)
                    {
                        handler.AddHandler(session.Session);
                    }
                }
            }
        }

        #endregion StartListening

        #region StopListening

        protected virtual void StopListening()
        {
            ((IVolumeControl)this).StopListening();
        }

        void IVolumeControl.StopListening()
        {
            if (isListening)
            {
                isListening = false;
                List<AudioSession> sessions = GetSessions();
                if (sessions.Count > 0)
                {
                    foreach (AudioSession session in sessions)
                    {
                        handler.RemoveHandler(session.Session);
                    }
                }
            }
        }

        #endregion StopListening

        #region UpdateSessions

        protected virtual void UpdateSessions()
        {
            ((IVolumeControl)this).UpdateSessions();
        }

        void IVolumeControl.UpdateSessions()
        {
            List<AudioSession> sessions = GetSessions();
            if (sessions.Count > 0)
            {
                foreach (AudioSession session in sessions)
                {
                    handler.RemoveHandler(session.Session);
                }
                foreach (AudioSession session in sessions)
                {
                    session.Session.SimpleAudioVolume.MasterVolume = _volume;
                    session.Session.SimpleAudioVolume.Mute = _mute;
                    handler.AddHandler(session.Session);
                }
            }
        }

        #endregion UpdateSessions

        #region Dispose

        protected virtual void Dispose()
        {
            ((IVolumeControl)this).Dispose();
        }

        void IVolumeControl.Dispose()
        {
            List<AudioSession> sessions = GetSessions();
            foreach (AudioSession session in sessions)
            {
                handler.RemoveHandler(session.Session);
            }
        }

        #endregion Dispose

        #endregion Interface

        #region Private Methods

        ////TODO:  CAN We Reuse this function somewhere else?
        //private bool IsProcessRunning(Process process)
        //{
        //    try { Process.GetProcessById(process.Id); }
        //    catch (InvalidOperationException) { return false; }
        //    catch (ArgumentException) { return false; }
        //    return true;
        //}

        private List<AudioSession> GetSessions()
        {
            List<AudioSession> controls = new List<AudioSession>();
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection deviceCollection = deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);
            string devName;
            for (int i = 0; i < deviceCollection.Count; i++)
            {
                MMDevice device = deviceCollection[i];
                devName = device.FriendlyName;
                for (int j = 0; j < device.AudioSessionManager.Sessions.Count; j++)
                {
                    AudioSessionControl session = device.AudioSessionManager.Sessions[j];
                    Process p;
                    try
                    {
                        p = Process.GetProcessById((int)session.ProcessID);
                    }
                    catch (InvalidOperationException)
                    {
                        p = Process.GetProcesses()[0];
                    }
                    catch (ArgumentException)
                    {
                        p = Process.GetProcesses()[0];
                    }

                    if (p.ProcessName == _processName)
                    {
                        controls.Add(new AudioSession(session, devName));
                    }
                }
            }
            return controls;
        }

        #endregion Private Methods

        #region Public Event Handlers

        public event VolumeChangedEventHandler VolumeChanged
        {
            add
            {
                {
                    handler.Handler.VolumeChanged += value;
                }
            }
            remove
            {
                {
                    handler.Handler.VolumeChanged -= value;
                }
            }
        }

        #endregion Public Event Handlers

        #region Private event handlers

        private void OnVolumeChanged(object sender, VolumeDataEventArgs data)
        {
            List<AudioSession> sessions = GetSessions();
            float newVolume = ((float)data.Volume / 100);
            if (newVolume > 1)
                newVolume = 1;
            if (newVolume < 0)
                newVolume = 0;
            foreach (AudioSession session in sessions)
            {
                if ((Math.Abs(session.Session.SimpleAudioVolume.MasterVolume - newVolume) > .10) || (session.Session.SimpleAudioVolume.Mute != data.Mute))
                {
                    session.Session.SimpleAudioVolume.Mute = data.Mute;
                    session.Session.SimpleAudioVolume.MasterVolume = newVolume;
                }
                handler.AddHandler(session.Session);
            }
            if (isListening)
            {
                if (handler.Handler.HasListeners)
                {
                    if (sender != this)
                    {
                        handler.Handler.RaiseEvent(this, data);
                    }
                }
            }
        }

        #endregion Private event handlers
    }
}