using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Spareio.Installer.Helper
{
    class FileHelper
    {
        public static string GetCurrentVersion(Assembly assembly)
        {
            string  currentVersion = null;
            try
            {
                //Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                 currentVersion = fvi.FileVersion;
            }
            catch (System.Exception)
            {
                throw;
            }
            return currentVersion;
        }
    }
}
