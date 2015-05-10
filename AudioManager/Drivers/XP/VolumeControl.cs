using CoreAudioApi;

/*
 * "On Windows XP, there wasn't really much you could do about it since there was a single volume control for all sounds
 * generated on the PC -- whether they came from Microsoft Word or Windows Media Player. This is just not a problem on
 * Windows Vista because we have replaced the old Volume Control with the new Volume Mixer," said Jim Allchin,
 * Microsoft Co-President, Platform and Services Division.
*/

namespace AudioManager.Drivers.XP
{
    sealed public class VolumeControl : IVolumeControl
    {
        #region Private Fileds

        private int oldVolume = 0;
        private int baseVolume;

        #endregion Private Fileds

        #region Constructor

        public VolumeControl(string _processName)
        {
            baseVolume = APIWrapper.GetVolume();
        }

        #endregion Constructor

        #region Interface

        #region CanListen

        bool IVolumeControl.CanListen
        {
            get { return false; }
        }

        #endregion CanListen

        #region SetVolume

        void IVolumeControl.SetVolume(int value)
        {
            APIWrapper.SetVolume(value);
        }

        #endregion SetVolume

        #region SetMute

        void IVolumeControl.SetMute(bool value)
        {
            if (value)
            {
                oldVolume = APIWrapper.GetVolume();
                APIWrapper.SetVolume(0);
            }
            else
            {
                if (oldVolume == 0)
                    oldVolume = 100;
                APIWrapper.SetVolume(oldVolume);
            }
        }

        #endregion SetMute

        #region GetVolume

        int IVolumeControl.GetVolume()
        {
            return APIWrapper.GetVolume();
        }

        #endregion GetVolume

        #region GetMute

        bool IVolumeControl.GetMute()
        {
            return APIWrapper.GetVolume() == 0;
        }

        #endregion GetMute

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
            APIWrapper.SetVolume(baseVolume);
        }

        #endregion Dispose

        #endregion Interface

        public event VolumeChangedEventHandler VolumeChanged { add { } remove { } }
    }
}