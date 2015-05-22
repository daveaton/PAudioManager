using CoreAudioApi;

namespace AudioManager.Drivers.Vista
{
    sealed public class VolumeControl : IVolumeControl
    {
        #region Private Fields

        private int baseVolume;
        private bool baseMute;

        #endregion Private Fields

        #region Constructor

        public VolumeControl(string processName)
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                this.baseVolume = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                this.baseMute = device.AudioEndpointVolume.Mute;
            }
        }

        #endregion Constructor

        #region Interface

        #region CanListen

        bool IVolumeControl.CanListen
        {
            get { return false; }
        }

        #endregion CanListen

        #region Set Volume

        void IVolumeControl.SetVolume(int value)
        {
            float newVolume = ((float)value / 100);
            if (newVolume > 1)
                newVolume = 1;
            if (newVolume < 0)
                newVolume = 0;
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume;
            }
        }

        #endregion Set Volume

        #region Set Mute

        void IVolumeControl.SetMute(bool value)
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                device.AudioEndpointVolume.Mute = value;
            }
        }

        #endregion Set Mute

        #region Get Volume

        int IVolumeControl.GetVolume()
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                return (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            }
        }

        #endregion Get Volume

        #region Get Mute

        bool IVolumeControl.GetMute()
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                return device.AudioEndpointVolume.Mute;
            }
        }

        #endregion Get Mute

        #region StartListening Dummy

        void IVolumeControl.StartListening()
        {
        }

        #endregion StartListening Dummy

        #region StopListening Dummy

        void IVolumeControl.StopListening()
        {
        }

        #endregion StopListening Dummy

        #region UpdateSessions Dummy

        void IVolumeControl.UpdateSessions()
        {
        }

        #endregion UpdateSessions Dummy

        #region Dispose

        void IVolumeControl.Dispose()
        {
            float newVolume = ((float)baseVolume / 100);
            if (newVolume > 1)
                newVolume = 1;
            if (newVolume < 0)
                newVolume = 0;
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            using (MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia))
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume;
                device.AudioEndpointVolume.Mute = this.baseMute;
            }
            
        }

        #endregion Dispose

        #endregion Interface

        public event VolumeChangedEventHandler VolumeChanged { add { } remove { } }
    }
}