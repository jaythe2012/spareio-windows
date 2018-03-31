using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Spareio.WinService.Helper
{
    public class ActiveDirectoryService
    {
        public static bool IsInDomain()
        {
            Win32.NetJoinStatus status = Win32.NetJoinStatus.NetSetupUnknownStatus;
            IntPtr pDomain = IntPtr.Zero;
            int result = Win32.NetGetJoinInformation(null, out pDomain, out status);
            if (pDomain != IntPtr.Zero)
            {
                Win32.NetApiBufferFree(pDomain);
            }
            if (result == Win32.ErrorSuccess)
            {
                bool voohoo = status == Win32.NetJoinStatus.NetSetupDomainName;
                return status == Win32.NetJoinStatus.NetSetupDomainName;
            }
            else
            {
                throw new Exception("Domain Info Get Failed", new Win32Exception());
            }
        }

        internal class Win32
        {
            public const int ErrorSuccess = 0;

            [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int NetGetJoinInformation(string server, out IntPtr domain, out NetJoinStatus status);

            [DllImport("Netapi32.dll")]
            public static extern int NetApiBufferFree(IntPtr Buffer);

            public enum NetJoinStatus
            {
                NetSetupUnknownStatus = 0,
                NetSetupUnjoined,
                NetSetupWorkgroupName,
                NetSetupDomainName
            }
        }
    }
}
