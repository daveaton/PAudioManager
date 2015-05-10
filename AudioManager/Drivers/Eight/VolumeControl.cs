using AudioManager.Helpers;
using CoreAudioApi;
using CoreAudioApi.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AudioManager.Drivers.Eight
{
    public class VolumeControl : IVolumeControl
    {

        #region Private Fields

        string _processName;
        bool isListening = false;
        SessionHandler handler;
        float _volume;
        bool _mute;

        #endregion

        #region Constructor

        public VolumeControl(string processName)
        {
            //Windows 8 groups the processes under TrafficBrowserTabs
            _processName = processName;
            handler = new SessionHandler();
            handler.Handler.VolumeChanged += OnVolumeChanged;
        }

        #endregion

        #region Interface

        #region CanListen

        bool IVolumeControl.CanListen
        {
            get { return true; }
        }

        #endregion

        #region Set Volume

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

        #endregion

        #region Set Mute

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
        
        #endregion

        #region Get Volume

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

        #endregion

        #region Get Mute

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

        #endregion

        #region StartListening

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

        #endregion

        #region StopListening

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
                        if (handler.IDs.Exists(id => id == session.Session.SessionInstanceIdentifier)) 
                        { 
                            session.Session.UnregisterAudioSessionNotification(handler.Handler);
                        }
                    }
                }
            }
        }

        #endregion

        #region UpdateSessions

        void IVolumeControl.UpdateSessions()
        {
            List<AudioSession> sessions = GetSessions();
            if (sessions.Count > 0)
            {
                if (isListening)
                {
                    foreach (AudioSession session in sessions)
                    {
                        handler.RemoveHandler(session.Session);
                    }
                }
                foreach (AudioSession session in sessions)
                {
                    session.Session.SimpleAudioVolume.MasterVolume = _volume;
                    session.Session.SimpleAudioVolume.Mute = _mute;
                    if (isListening)
                    {
                        handler.AddHandler(session.Session);
                    }
                }
            }

        }

        #endregion

        #region Dispose

        void IVolumeControl.Dispose()
        {
            List<AudioSession> sessions = GetSessions();
            foreach (AudioSession session in sessions)
            {
                handler.RemoveHandler(session.Session);
            }
        }

        #endregion

        #endregion

        #region Private Methods

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
                    catch
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

        #endregion

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

        #endregion

        #region Private event handlers

        void OnVolumeChanged(object sender, VolumeData data)
        {
            List<AudioSession> sessions = GetSessions();
            float newVolume = ((float)data.Volume / 100);
            if (newVolume > 1)
                newVolume = 1;
            if (newVolume < 0)
                newVolume = 0;
            foreach (AudioSession session in sessions)
            {
                if ((session.Session.SimpleAudioVolume.MasterVolume != newVolume) || (session.Session.SimpleAudioVolume.Mute != data.Mute))
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

        #endregion

    }
}
