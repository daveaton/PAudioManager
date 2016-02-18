namespace AudioManager.Helpers
{
    public static class SysInfo
    {
        public static OsVersion Os
        {
            get
            {
                System.Version info = System.Environment.OSVersion.Version;
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
                return System.IntPtr.Size == 8;
            }
        }

        public static bool Is64BitsSystem
        {
            get
            {
                if ((System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor >= 1) ||
        System.Environment.OSVersion.Version.Major >= 6)
                {
                    using (System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess())
                    {
                        bool retVal;
                        if (!NativeMethods.IsWow64Process(p.Handle, out retVal))
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
                return System.Environment.OSVersion.VersionString;
            }
        }

        public static string ServicePack
        {
            get
            {
                return System.Environment.OSVersion.ServicePack;
            }
        }
    }
}