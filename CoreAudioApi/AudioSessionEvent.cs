using CoreAudioApi.Helpers;
using CoreAudioApi.Interfaces;
using System;

namespace CoreAudioApi
{
    public delegate void VolumeChangedEventHandler(object sender, VolumeDataEventArgs data);

    public class AudioSessionEvent : IAudioSessionEvents
    {
        public event VolumeChangedEventHandler VolumeChanged, data;

        public bool HasListeners
        {
            get
            {
                return VolumeChanged != null;
            }
        }

        public void RaiseEvent(object sender, VolumeDataEventArgs data)
        {
            if (sender != null && data !=null)
            {
                VolumeChangedEventHandler onVolumeChanged = this.VolumeChanged;
                if (onVolumeChanged != null)
                {
                    onVolumeChanged(sender, data);
                }
            }
        }

        public int OnDisplayNameChanged(string NewDisplayName, Guid EventContext)
        {
            return 0;
        }

        public int OnIconPathChanged(string NewIconPath, Guid eventContext)
        {
            return 0;
        }

        public int OnSimpleVolumeChanged(float NewVolume, bool newMute, Guid eventContext)
        {
            if (VolumeChanged != null)
            {
                VolumeDataEventArgs data = new VolumeDataEventArgs((int)(NewVolume * 100), newMute, eventContext);
                RaiseEvent(this, data);
            }
            return 0;
        }

        public int OnChannelVolumeChanged(uint ChannelCount, IntPtr NewChannelVolumeArray, uint ChangedChannel, Guid EventContext)
        {
            return 0;
        }

        public int OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext)
        {
            return 0;
        }

        public int OnStateChanged(AudioSessionState NewState)
        {
            return 0;
        }

        public int OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason)
        {
            return 0;
        }
    }
}