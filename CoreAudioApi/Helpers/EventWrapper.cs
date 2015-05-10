namespace CoreAudioApi.Helpers
{
    public class EventWrapper
    {
        public AudioSessionEvent EventHandler { get; set; }

        public string SessionId { get; set; }

        public uint ProcessId { get; set; }
    }
}