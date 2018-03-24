using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;


namespace Spareio.UI.Utils
{
    [SuppressUnmanagedCodeSecurityAttribute]
    public class InternetConnectionChecker
    {
        // Extern Library
        // UnManaged code - be careful.
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool
            InternetGetConnectedState(out int Description, int ReservedValue);

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetCheckConnection(string lpszUrl, int dwFlags, int dwReserved);
        const int FLAG_ICC_FORCE_CONNECTION = 1;

        /// <summary>
        /// Determines if there is an active connection on this computer
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedToInternet()
        {
            bool hasActiveConnection = false;

            try
            {
                hasActiveConnection = (InternetCheckConnection("http://www.google.com", FLAG_ICC_FORCE_CONNECTION, 0) ||
                                       InternetCheckConnection("http://www.microsoft.com", FLAG_ICC_FORCE_CONNECTION, 0) ||
                                       InternetCheckConnection("http://www.facebook.com", FLAG_ICC_FORCE_CONNECTION, 0));
            }
            catch (DllNotFoundException)
            {
                //-- Wininet.dll might somehow be missing on user's machine.
                return true;
            }
            catch (Exception)
            {
                return false;
            }

            return hasActiveConnection;
        }
    }
}
