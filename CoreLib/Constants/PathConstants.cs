using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreLib
{
    public class PathConstants
    {
        private static string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Spareio");

        private static string programDataLogPath = Path.Combine(programDataPath, "Logs");
        private static string programDataConfigPath = Path.Combine(programDataPath, "Config");

        public static string GetLogPath()
        {
            try
            {
                if (!Directory.Exists(programDataLogPath))
                {
                    Directory.CreateDirectory(programDataLogPath);
                }
            }
            catch (Exception ex)
            {
            }
            return programDataLogPath;
        }

        public static string GetConfigPath()
        {
            try
            {
                if (!Directory.Exists(programDataConfigPath))
                {
                    Directory.CreateDirectory(programDataConfigPath);
                }
            }
            catch (Exception ex)
            {
            }
            return programDataConfigPath;
        }
    }
}
