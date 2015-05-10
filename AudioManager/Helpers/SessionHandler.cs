using CoreAudioApi;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioManager.Helpers
{
    public class SessionHandler
    {
        public SessionHandler()
        {
            Handler = new AudioSessionEvent();
            IDs = new List<string>();
        }

        public AudioSessionEvent Handler { get; private set; }

        private List<string> IDs;

        public void AddHandler(AudioSessionControl session)
        {
            string currentId = session.SessionInstanceIdentifier + session.ProcessID;
            if (!IDs.Exists(id => id == currentId))
            {
                session.RegisterAudioSessionNotification(Handler);
                IDs.Add(currentId);
            }
        }

        public void RemoveHandler(AudioSessionControl session)
        {
            string currentId = session.SessionInstanceIdentifier + session.ProcessID;
            if (IDs.Exists(id => id == currentId))
            {
                try
                {
                    session.UnregisterAudioSessionNotification(Handler);
                    IDs.Remove(currentId);
                }
                catch (COMException)
                {
                    //The UnregisterAudioSessionNotification failed trying to remove subscriptor due to invalid handler
                    //AKA not subscribed to this session
                    //Why we end up here?? the sessionId should not be on the list
                    //The AudioManager is being disposed. DO NOTHING!!
                }
            }
        }
    }
}