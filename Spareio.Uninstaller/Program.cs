using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Spareio.Uninstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Removing spareio.......");
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string installerPath = Path.Combine(assemblyPath, "SpareioInstaller.exe");

            var process = Process.Start(installerPath, " --silent --uninstall --invoked");
            process.WaitForExit();

            Console.WriteLine("Successfully removed");
        }
    }
}
