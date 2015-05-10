using CoreAudioApi;

namespace AudioManager.Drivers
{
    public interface IVolumeControl
    {
        bool CanListen { get; }

        void SetVolume(int value);

        void SetMute(bool value);

        int GetVolume();

        bool GetMute();

        void StartListening();

        void StopListening();

        void UpdateSessions();

        void Dispose();

        event VolumeChangedEventHandler VolumeChanged;
    }
}