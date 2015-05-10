using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioManager.Helpers
{
    public static class SysInfo
    {
        public static OsVersion Os
        {
            get
            {
                Version info = Environment.OSVersion.Version;
                OsVersion version = default(OsVersion);
                if (info.Major == 6)
                {
                    if (info.Minor == 0)
                        version = OsVersion.Vista;
                    else if (info.Minor == 1)
                        version = OsVersion.Seven;
                    else if (info.Minor > 1)
                        version = OsVersion.Eight;
                }
                else if (info.Major > 6)
                {
                    version = OsVersion.Eight;
                }
                else
                {
                    version = OsVersion.XP;
                }
                return version;
            }
        }

        public static bool Is64BitsProcess
        {
            get
            {
                return IntPtr.Size == 8;
            }
        }

        public static bool Is64BitsSystem
        {
            get
            {
                if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
        Environment.OSVersion.Version.Major >= 6)
                {
                    using (Process p = Process.GetCurrentProcess())
                    {
                        bool retVal;
                        if (!IsWow64Process(p.Handle, out retVal))
                        {
                            return false;
                        }
                        return retVal;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static string FullSystemName
        {
            get
            {
                return Environment.OSVersion.VersionString;
            }
        }

        public static string ServicePack
        {
            get
            {
                return Environment.OSVersion.ServicePack;
            }
        }

        #region Private Methods

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        #endregion Private Methods
    }
}