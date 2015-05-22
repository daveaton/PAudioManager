using CoreAudioApi;
using System;

namespace AudioManager.Drivers.XP
{
    internal static class APIWrapper
    {
        public static void SetVolume(int value)
        {
            if (value < 0)
                value = 0;

            if (value > 100)
                value = 100;

            try
            {
                // Calculate the volume that's being set
                double newVolume = ushort.MaxValue * value / 10.0;

                uint v = ((uint)newVolume) & 0xffff;
                uint vAll = v | (v << 16);

                // Set the volume
                NativeMethods.WaveOutSetVolume(IntPtr.Zero, vAll);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int GetVolume()
        {
            int result = 100;
            //int result2 = 100;
            try
            {
                MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                using (MMDevice device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole))
                {
                    result = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                }
                //result2 = (int)(device2.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}