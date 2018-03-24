using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;

namespace Spareio.Installer.Utils
{
    internal class InstallUtils
    {
        static readonly string UninstallRegKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        internal static string Company { get { return "Spareio"; } }
        internal static string Product { get { return "SpareioApp"; } }
        internal static string WcProcessName { get { return "Spareio.exe"; } }
        internal static string WcPackageName { get { return "Spareio.zip"; } }
        internal static string WcInstaller { get { return "WcInstaller.exe"; } }

        //for file explorer use only
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const int WM_USER = 0x0400; //http://msdn.microsoft.com/en-us/library/windows/desktop/ms644931(v=vs.85).aspx

        internal static Int64 GetMillisecondsSinceEpoch()
        {
            var netTime = DateTime.UtcNow;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (Int64)(netTime - epoch).TotalMilliseconds;
        }

        internal static string GetSelfVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

       

        private static T GetRegKeyValue<T>(RegistryKey main, string key, string name)
        {
            using (var reg = main.OpenSubKey(key))
            {
                return (T)reg.GetValue(name, default(T), RegistryValueOptions.None);
            }
        }

        internal static string GetTimeZone()
        {
            try
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;
                string timeZone = localZone.StandardName;
                return timeZone;
            }
            catch (System.Exception ex)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Deleting Start menu icons in Windows 8 and 8.1 requires a restart of file explorer to get rid of tiles.
        /// </summary>
        /*internal static void RestartWindowsExplorer() 
        {
            string osVersion = GetOSVersion();
            if (osVersion.Equals("Win8/Win2012Server") || osVersion.Equals("Win8.1/Win2012Server R2"))
            {
                try
                {
                    var ptr = FindWindow("Shell_TrayWnd", null);
                    PostMessage(ptr, WM_USER + 436, (IntPtr)0, (IntPtr)0);

                    do
                    {
                        ptr = FindWindow("Shell_TrayWnd", null);
                        Console.WriteLine("PTR: {0}", ptr.ToInt32());

                        if (ptr.ToInt32() == 0)
                        {
                            break;
                        }

                        Thread.Sleep(1000);
                    } while (true);
                }
                catch (System.Exception ex)
                {
                    
                }
                string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
                Process process = new Process();
                process.StartInfo.FileName = explorer;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }*/

        internal static string RemoveUnInstaller()
        {
            string installedDate = String.Empty;

            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(
                UninstallRegKeyPath, true))
            {
                if (parent == null)
                {
                    throw new System.Exception("Uninstall registry key not found.");
                }
                try
                {

                    var allKeys = parent.GetSubKeyNames();
                    foreach (var k in allKeys)
                    {
                        bool bDelete = false;


                        using (var key = parent.OpenSubKey(k))
                        {
                            if (key != null)
                            {
                                bDelete = (string)key.GetValue("DisplayName") == Product;
                                if (bDelete)
                                {
                                    installedDate = (string)key.GetValue("InstallDate");
                                }
                            }
                        }

                        if (bDelete)
                        {
                            parent.DeleteSubKey(k);

                            // we can exit here, but we will keep looping just in case there are more than one WC here
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(
                        "An error occurred deleting uninstall information from the registry.",
                        ex);
                }

                return installedDate;
            }
        }

        internal static string CreateUninstaller(string installedDate = null)
        {
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(
                UninstallRegKeyPath, true))
            {
                if (parent == null)
                {
                    throw new System.Exception("Uninstall registry key not found.");
                }
                try
                {
                    RegistryKey key = null;

                    try
                    {
                        string guidText = Guid.NewGuid().ToString("B");
                        key = parent.OpenSubKey(guidText, true) ??
                              parent.CreateSubKey(guidText);

                        if (key == null)
                        {
                            throw new System.Exception(String.Format("Unable to create uninstaller '{0}\\{1}'", UninstallRegKeyPath, guidText));
                        }

                        var v = FileVersionInfo.GetVersionInfo(Path.Combine(GetWcRunFolder(), "Spareio.exe"));
                       // var v = "1.0.0.0";

                        key.SetValue("DisplayName", Product);
                        key.SetValue("ApplicationVersion", v.ToString());
                        key.SetValue("Publisher", Company);
                        key.SetValue("DisplayIcon", GetPathToIcon());
                        key.SetValue("DisplayVersion", v.ToString());
                        key.SetValue("URLInfoAbout", "https://www.spare.io");
                        key.SetValue("Contact", "support@spare.io");
                        // during the upgrade we do not want to change the date due to active features based on install date
                        key.SetValue("InstallDate", installedDate ?? DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("UninstallString", GetPathToInstallerExe() + " --silent --uninstall");

                        return v.ToString();
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(
                        "An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.",
                        ex);
                }
            }
        }

        public static string GetPathToExe()
        {
            return Path.Combine(GetWcRunFolder(), "Spareio.exe");
        }

        public static string GetPathToInstallerExe()
        {
            return Path.Combine(GetWcRunFolder(), "SpareioInstaller.exe");
        }

        public static string GetPathToIcon()
        {
            return Path.Combine(GetWcRunFolder(), "xRewardIcon.ico");
        }

        public static string GetProgramFilesFolder()
        {
            string strInstallPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (String.IsNullOrEmpty(strInstallPath))
            {
                strInstallPath = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return strInstallPath;
        }
        internal static string GetInstallFolder()
        {
            string[] wc = { Company, Product };

            return Path.Combine(GetProgramFilesFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
        }
        internal static string GetWcRunFolder()
        {
            //string[] wc = { "Application" };
            //return Path.Combine(GetInstallFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
            string[] wc = { Company, Product };

            return Path.Combine(GetProgramFilesFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
        }
        internal static string GetWcDebugRunFolder()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string[] seperator = { "Installer" };
            string[] paths = dir.Split(seperator, StringSplitOptions.None);
            string path = paths[0] + "_build\\x86\\Debug\\";
            return path;
        }
       
        internal static string GetLavaProgramFilesFolder()
        {
            string[] wc = { Company };
            return Path.Combine(GetInstallFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
        }
        internal static string GetLavasoftAppDataFolder()
        {
            var AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = String.Format(@"{0}/{1}", AppData, Company);

            return folder;
        }
        public static string GetLavasoftProgramDataFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var folder = String.Format(@"{0}/{1}", programdata, Company);

            return folder;
        }
       
        internal static bool Is64BitOs()
        {
            return !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
        }

        internal static string GetOsBit()
        {
            return Is64BitOs() ? "64" : "32";
        }

        internal static bool IsWcInstalled()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            int installed = 0;

            // this should create installed 
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
            {
                if (parent != null)
                {
                    var obj = parent.GetValue("Installed");
                    if (obj != null)
                        installed = (int)obj;
                }
            }

            return installed != 0;
        }

        internal static void RemoveInstalledMark()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

            // this should create installed 
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name, true))
            {
                if (parent != null)
                {
                    try
                    {
                        // https://lavasoft.atlassian.net/browse/WC-2019 It is decided to keep installed entry with value 0
                        parent.SetValue("Installed", 0);
                    }
                    catch (System.ArgumentException ex)
                    {
                        // ignore the case when this value doesn't exist
                    }
                }
            }
        }

        internal static void GenerateSuccessfullInstallMark()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            int installed = 0;

            // this should create installed 
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
            {
                if (parent != null)
                {
                    var obj = parent.GetValue("Installed");
                    if (obj != null)
                        installed = (int)obj;
                }
            }

            if (installed == 0)
            {
                installed = 1;

                // write this machine id into the registry
                using (RegistryKey parent = Registry.LocalMachine.CreateSubKey(key_name))
                {
                    parent.SetValue("Installed", installed);
                }
            }
        }

        internal static string GenerateMachineIdGuid()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            string machineId = String.Empty;

            // in case of installing on the machine that already has machine id, we will re-use it
            // this is done in order to prevant frauds by installing continuously on the same machine
            // uninstall should never remove this key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
            {
                if (parent != null)
                {
                    machineId = (string)parent.GetValue("MachineId");
                }
            }

            if (String.IsNullOrEmpty(machineId))
            {
                //machineId = Guid.NewGuid().ToString("D");
                machineId = FingerPrint.Value();
                // write this machine id into the registry
                using (RegistryKey parent = Registry.LocalMachine.CreateSubKey(key_name))
                {
                    parent.SetValue("MachineId", machineId);
                }
            }

            return machineId;
        }

        internal static void SetIgnoreHms()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

            // in case of installing on the machine that already has machine id, we will re-use it
            // this is done in order to prevant frauds by installing continuously on the same machine
            // uninstall should never remove this key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name, true))
            {
                if (parent != null)
                {
                    parent.SetValue("SkipHms", 1);
                }
            }
        }

        internal static bool GetIgnoreHms()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

            // in case of installing on the machine that already has machine id, we will re-use it
            // this is done in order to prevant frauds by installing continuously on the same machine
            // uninstall should never remove this key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name, true))
            {
                if (parent != null)
                {
                    var res = parent.GetValue("SkipHms");
                    if (res != null)
                        return (int)res == 1;
                }
            }

            return false;
        }

        internal static string RemoveMachineIdGuid()
        {
            var key_name_lavasoft = String.Format(@"SOFTWARE\{0}", Company);
            var key_name_WC = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            string machineId = String.Empty;

            //In case we need machineID for a reinstall.
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name_WC))
            {
                if (parent != null)
                {
                    machineId = (string)parent.GetValue("MachineId");
                }
            }
            //delete complete Spareio key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name_lavasoft, true))
            {
                if (parent != null)
                {
                    try
                    {
                        parent.DeleteSubKeyTree(Product);
                    }
                    catch (System.ArgumentException ex)
                    {
                        // ignore the case when this value doesn't exist
                    }
                }
            }

            return machineId;
        }

        internal static string GenerateInstallIdGuid()
        {
            string installId = String.Empty;
            var folder = GetOptionsFolder();
            var Statistics = String.Format(@"{0}/Statistics.txt", folder);

            // try read statistics file
            if (File.Exists(Statistics))
            {
                // read and parse existing guid
                var content = File.ReadAllText(Statistics);
                var candidates = content.Split('"');
                foreach (var candidate in candidates)
                {
                    try
                    {
                        new Guid(candidate);

                        installId = candidate;
                        break;
                    }
                    catch
                    {
                        // skip non guid element
                    }
                }
            }
            else
            {
                // generate instal id and write it to statistics file
                installId = Guid.NewGuid().ToString("D");

                // create file
                Directory.CreateDirectory(folder);
                SetAllowEveryone(folder);

                File.WriteAllText(Statistics, String.Format(@"{{ ""install_id"" : ""{0}""}}", installId));
            }

            return installId;
        }

        public static void SetAllowEveryone(string path)
        {
            var drInfo = new DirectoryInfo(path);

            var sec = drInfo.GetAccessControl();

            // Using this instead of the "Everyone" string means we work on non-English systems.
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, AccessControlType.Allow));
            drInfo.SetAccessControl(sec);
        }

        public static string GetOptionsFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var folder = String.Format(@"{0}/{1}/{2}/Options", programdata, Company, Product);

            return folder;
        }

        public static string GetAppDataOptionsFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Lavasoft\Web Companion\Options");
        }

        public static string GetStartMenuPath()
        {
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Spareio\SpareioApp";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 5)
            {
                startMenuPath = @"C:\Documents and Settings\All Users\Start Menu\Programs\Spareio\SpareioApp";
            }
            return startMenuPath;
        }

        public static bool IsVistaOrHigher()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6;
        }

        public static string GetOSVersion()
        {
            // Getting OS service pack info. Returns an empty string in case no service pack installed
            string servicePack = Environment.OSVersion.ServicePack;

            // Getting OS Name by querying managemene object
            var osName = (from x in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>()
                select x.GetPropertyValue("Caption")).FirstOrDefault();
            return osName != null ? osName.ToString() + servicePack : "Unknown";
        }

        internal static string GetWCProgramDataFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var folder = String.Format(@"{0}/{1}", programdata, Company);

            return folder;
        }

        internal static string GetLocalAppDataFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folder = String.Format(@"{0}/{1}/", programdata, Company);

            return folder;
        }

        internal static string GetRoamingAppDataWCFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = String.Format(@"{0}/{1}/{2}", programdata, Company, Product);


            return folder;
        }

        internal static string GetHmsInstallFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var folder = String.Format(@"{0}/{1}", programdata, "AdAware");
            return folder;
        }

        internal static string GetRoamingAppDataLavasoftFolder()
        {
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = String.Format(@"{0}/{1}", programdata, Company);

            return folder;
        }

        internal static string GetPlatform(params string[] args)
        {
            string platform = "internal"; // "prod" is another option

            var argumentProcessor = new Utils.CmdLineArgs(args);

            // check platform base on the name of installer
            if (argumentProcessor.CheckArg("prod"))
            {
                platform = "prod";
            }
            else if (argumentProcessor.CheckArg("internal"))
            {
                platform = "internal";
            }
            else if (argumentProcessor.CheckArg("update"))
            {
                // in case of update we default this to prod https://lavasoft.atlassian.net/browse/WC-1972
                platform = "prod";
            }
            else
            {
                var myName = Process.GetCurrentProcess().ProcessName;
                if (myName.EndsWith("-prod"))
                {
                    platform = "prod";
                }
            }

            return platform;
        }

        public static bool IsDirectoryEmpty(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            return dirs.Length == 0 && files.Length == 0;
        }

        public static bool IsDirectoryEmptyPlusOne(string path)
        {
            if (Directory.Exists(path))
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                return dirs.Length == 1 && files.Length == 0;
            }

            return true;
        }

        /// <summary>
        /// this method checks if the path to folder be deleted should only be theone passed as argument or, 
        /// when allowed, if parent folder can be deleted instead.
        /// WARNING: use this method when you are sure the parent folder can be deleted.
        /// </summary>
        /// <param name="regularPath">the path to a folder to be deleted</param>
        /// <returns>the path to directory that can be deleted</returns>
        public static string fullPathToBeDeleted(string regularPath)
        {
            string parentPath = Directory.GetParent(regularPath).FullName;
            if (IsDirectoryEmptyPlusOne(parentPath))//this means only web companion is in Lavasoft program files folder, there is no Ad-aware product installed
            {
                return parentPath;
            }
            else
            {
                return regularPath;
            }
        }

        

        public delegate void SafeCallFunction();
        public static void SafeCall(SafeCallFunction func)
        {
            try
            {
                func();
            }
            catch
            {

            }
        }
        public static void SafeCallInterruptable(bool bInterruptOnError, SafeCallFunction func)
        {
            try
            {
                func();
            }
            catch (System.Exception ex)
            {
                if (bInterruptOnError)
                    throw new System.Exception();
            }
        }
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        internal static bool IsConnectionUp()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.webcompanion.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // reference http://neophob.com/2010/03/wmi-query-windows-securitycenter2/
        enum _WSC_AV_INFO
        {
            [Text("Firewall")]
            WSC_SECURITY_PROVIDER_FIREWALL = 0x010000,
            [Text("Autoupdate")]
            WSC_SECURITY_PROVIDER_AUTOUPDATE_SETTINGS = 0x020000,
            [Text("Antivirus")]
            WSC_SECURITY_PROVIDER_ANTIVIRUS = 0x040000,
            [Text("Antispyware")]
            WSC_SECURITY_PROVIDER_ANTISPYWARE = 0x080000,
            [Text("Internet")]
            WSC_SECURITY_PROVIDER_INTERNET_SETTINGS = 0x100000,
            [Text("UAC")]
            WSC_SECURITY_PROVIDER_USER_ACCOUNT_CONTROL = 0x200000,
            [Text("Security")]
            WSC_SECURITY_PROVIDER_SERVICE = 0x400000,
            [Text("No security")]
            WSC_SECURITY_PROVIDER_NONE_MASK = 0xFF0000,
            [Text("Scanner is off")]
            WCS_SCANNER_UNKNOWN_MASK = 0x00FF00,
            [Text("Scanner is on")]
            WCS_SCANNER_RUNNING = 0x001000,
            [Text("Defs are too old")]
            WCS_TOO_OLD = 0x000010,
            [Text("Defs are u2d")]
            WCS_UP_TO_DATE_MASK = 0x0000FF
        }

        private class TextAttribute : Attribute
        {
            public string Text { set; get; }
            public TextAttribute(string txt)
            {
                Text = txt;
            }
        }

        private static TextAttribute GetAttributeOfType(Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(TextAttribute), false);
            return (attributes.Length > 0) ? (TextAttribute)attributes[0] : null;
        }

        public static string GetAvInfoFromState(int state)
        {
            var info = new StringBuilder();


            foreach (var value in Enum.GetValues(typeof(_WSC_AV_INFO)))
            {
                var iValue = (int)value;
                var name = ((_WSC_AV_INFO)value).ToString();
                var txt = GetAttributeOfType((_WSC_AV_INFO)value).Text;
                if (name.EndsWith("_MASK"))
                {
                    if ((state & iValue) == 0)
                        info.Append(txt + ",");
                }
                else
                {
                    if ((state & iValue) != 0)
                        info.Append(txt + ",");
                }

            }



            return info.ToString();
        }

        public static void ReportInstalledAntivirus()
        {
            bool vista = IsVistaOrHigher();
            string wmipathstr = @"\\" + Environment.MachineName + (vista ? @"\root\SecurityCenter2" : @"\root\SecurityCenter");
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                if (instances.Count > 0)
                {
                    foreach (var instance in instances)
                    {
                        String str = String.Empty;
                        if (vista)
                        {
                            var state = Int32.Parse(instance["productState"].ToString());

                            str = String.Format("Antivirus detected: {0} [{1}]", instance["displayName"],
                                GetAvInfoFromState(state));
                        }
                        else
                        {
                            str = String.Format("Antivirus detected: {0} [{1}]", instance["displayName"],
                                instance["companyName"]);
                        }
                        CommunicationUtils.SendReport(str);
                    }
                }
                else
                {
                    CommunicationUtils.SendReport("Antivirus not detected");
                }
            }
            catch (System.Exception e)
            {
                CommunicationUtils.SendReport("Antivirus: failed to detect: " + e);
                Trace.WriteLine(e.Message);
            }

        }

        internal static void SaveRevertSettingsInfo(int reverthp, int revertse)
        {

            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);


            // write this machine id into the registry
            using (RegistryKey parent = Registry.LocalMachine.CreateSubKey(key_name))
            {
                parent.SetValue("RevertHP", reverthp);
                parent.SetValue("RevertSE", revertse);
            }


        }

        internal static void ReportIsMachineVM()
        {
            bool isVM = false;
            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            isVM = true;
                        }
                    }
                }
            }
            CommunicationUtils.SendReport("vm_check " + isVM);
        }

        public static bool IsHmsInstalled()
        {
            return Directory.Exists(Path.Combine(InstallUtils.GetProgramFilesFolder(), @"AdAware\hms"));
        }

      

        internal static void ReportInstallFlag()
        {
            try
            {
                bool isWCRegistryExist = false;

                var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
                int installed = 0;

                // this should create installed 
                using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
                {
                    if (parent != null)
                    {
                        var obj = parent.GetValue("Installed");
                        if (obj != null)
                        {
                            installed = (int)obj;
                            isWCRegistryExist = true;
                        }
                    }
                }
                CommunicationUtils.SendReport("reg_check :" + isWCRegistryExist);
            }
            catch (System.Exception ex)
            {
                CommunicationUtils.SendReport("reg_check :false");
            }
        }

        public static void UpdatePartnerInfo(string partnerId, string campaignId, string token)
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

            using (RegistryKey parent = Registry.LocalMachine.CreateSubKey(key_name))
            {
                if (parent != null)
                {
                    parent.SetValue("PartnerId", partnerId);
                    parent.SetValue("CampaignId", campaignId);
                    parent.SetValue("xToken",token);
                    parent.SetValue("installDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"));
                }
            }
        }

        internal static string ReadMachineGuid()
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            string machineId = String.Empty;

            // in case of installing on the machine that already has machine id, we will re-use it
            // this is done in order to prevant frauds by installing continuously on the same machine
            // uninstall should never remove this key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
            {
                if (parent != null)
                {
                    machineId = (string) parent.GetValue("MachineId");
                }
            }
            return machineId;
        }

        internal static string ReadValue(string key)
        {
            var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            string value = String.Empty;

            // in case of installing on the machine that already has machine id, we will re-use it
            // this is done in order to prevant frauds by installing continuously on the same machine
            // uninstall should never remove this key
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
            {
                if (parent != null)
                {
                    value = (string)parent.GetValue(key) != null ? (string)parent.GetValue(key) : String.Empty;
                }
            }
            return value;
        }
    }
}
