using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AudioManager.Helpers
{
    public static class ProcessInfo
    {
        public static int Id
        {
            get
            {
                return AppDomain.CurrentDomain.Id;
            }
        }

        public static string Name
        {
            get
            {
                return AppDomain.CurrentDomain.FriendlyName;
            }
        }

        public static Guid SessionId
        {
            get
            {
                return new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute), false)).Value.ToUpper());
            }
        }
    }
}