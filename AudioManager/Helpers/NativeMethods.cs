using System;

namespace AudioManager.Helpers
{
    internal sealed class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool IsWow64Process(
            [System.Runtime.InteropServices.In] IntPtr hProcess,
            [System.Runtime.InteropServices.Out] out bool wow64Process
        );
    }
}