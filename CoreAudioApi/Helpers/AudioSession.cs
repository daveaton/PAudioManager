namespace CoreAudioApi.Helpers
{
    public class AudioSession
    {
        public AudioSession(AudioSessionControl session, string deviceName)
        {
            Session = session;
            DeviceName = deviceName;
        }

        public AudioSessionControl Session { get; private set; }

        public string DeviceName { get; private set; }
    }
}