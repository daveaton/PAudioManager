using System;
using System.Runtime.InteropServices;

namespace AudioManager.Drivers.XP
{
    internal static class NativeMethods
    {
        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);
    }
}