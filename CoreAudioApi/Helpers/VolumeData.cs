using System;

namespace CoreAudioApi.Helpers
{
    public class VolumeDataEventArgs : EventArgs
    {
        public VolumeDataEventArgs(int volume, bool mute, Guid eventContext)
        {
            Volume = volume;
            Mute = mute;
            EventContext = eventContext;
        }

        public int Volume { get; private set; }

        public bool Mute { get; private set; }

        public Guid EventContext { get; private set; }
    }
}