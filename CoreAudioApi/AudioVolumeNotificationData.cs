/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;

namespace CoreAudioApi
{
    public class AudioVolumeNotificationData
    {
        private readonly Guid eventContext;
        private readonly bool muted;
        private readonly float masterVolume;
        private readonly int channels;
        private readonly float[] channelVolume;

        public Guid EventContext
        {
            get
            {
                return eventContext;
            }
        }

        public bool Muted
        {
            get
            {
                return muted;
            }
        }

        public float MasterVolume
        {
            get
            {
                return masterVolume;
            }
        }

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        public float[] GetChannelVolume()
        {
            return channelVolume;
        }

        public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume)
        {
            this.eventContext = eventContext;
            this.muted = muted;
            this.masterVolume = masterVolume;
            channels = channelVolume.Length;
            this.channelVolume = channelVolume;
        }
    }
}