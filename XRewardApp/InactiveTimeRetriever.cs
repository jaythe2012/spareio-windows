using System;
using System.Runtime.InteropServices;

namespace Spareio.UI
{
    public class InactiveTimeRetriever
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static TimeSpan? GetInactiveTime()
        {
            LASTINPUTINFO info = new LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            if (GetLastInputInfo(ref info))
                return TimeSpan.FromMilliseconds(Environment.TickCount - info.dwTime);
            else
                return null;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}


