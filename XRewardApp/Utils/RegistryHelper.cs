using System;
using CoreLib;
using Microsoft.Win32;

namespace Spareio.UI.Utils
{
    class RegistryHelper
    {
        internal static string Company { get { return "Spareio"; } }
        internal static string Product { get { return "SpareioApp"; } }

        public static string GetValue(string key)
        {
            string hashToken = String.Empty;
            try
            {
                var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

                using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
                {
                    if (parent != null)
                    {
                        hashToken = (string)parent.GetValue(key);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while reading xToken from registry" +ex.Message);
            }

            return hashToken;






        }
    }
}
