using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Windows;
using CoreLib;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Spareio.UI.Constants;
using Spareio.UI.Model;
using Spareio.UI.SpareioClient;
using Spareio.UI.Utils;
using Spareio.UI.WcfHost;

namespace Spareio.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static List<string> commandLineArgs;
        private System.Timers.Timer _checkForSoftwareUpdateTimer = null;


        public static List<string> CommandLineArgs
        {
            get { return commandLineArgs; }
            set { commandLineArgs = value; }
        }

        private static string executableLnk { get { return "Spareio.lnk"; } }


        public App()
        {
            try
            {

                _checkForSoftwareUpdateTimer = new System.Timers.Timer((AppConstant.AfterStartupUpdateDelay).TotalMilliseconds);
                _checkForSoftwareUpdateTimer.Elapsed += CheckForUpdatesTimer_Elapsed;
                _checkForSoftwareUpdateTimer.Start();

            }
            catch (Exception ex)
            {
                LogWriter.Error("Error in App constructor "+ex.Message);

                //throw;
            }
        }

        [System.STAThreadAttribute]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            try
            {
                EnableLogging();
                OpenWcfHost();
                CommandLineArgs = Environment.GetCommandLineArgs().OfType<string>().ToList();
                var cmdArgs = new CmdLineArgs(Environment.GetCommandLineArgs());
                //LogWriter.Info("CommandLine" + cmdArgs.ToString());

                if (CommandLineArgs.Contains(AppConstant.installCmd) || CommandLineArgs.Contains(AppConstant.installCmdDefault))
                {
                    LogWriter.Info("Contains install cmd");
                    CreateShortcut();
                    RunOnStartUp(true);
                    System.Windows.Application.Current.Shutdown();
                }
                if (CommandLineArgs.Contains("--afterinstall"))
                {
                    string url = BuildLaunchUrl();
                    Process.Start(url);
                }
                LogWriter.Info("Time to start App");
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while starting an app "+ex.Message);
            }
        }

        private static string BuildLaunchUrl()
        {
            string url = "https://www.adaware.com/myadaware/aa-pro-offer/";
            url = url + "?mkey=" + RegistryHelper.GetValue("MachineId");
           
            return url;
        }

        private static void EnableLogging()
        {
            try
            {
                LogWriter.Enable = true;
                LogWriter.Initialize("Spareio");
            }
            catch (Exception ex)
            {
            }
        }

        private static void OpenWcfHost()
        {
            try
            {
                LogWriter.Info("OpenWcfHost: " );

                Uri httpBaseAddress = new Uri("http://localhost:9007/spareio");
                //Instantiate ServiceHost
                var localHost = new ServiceHost(typeof(LocalyHostedService),
                    httpBaseAddress);
                //var binding = new BasicHttpBinding();
                var binding = new WebHttpBinding();
                //Add Endpoint to Host
                localHost.AddServiceEndpoint(typeof(ILocalyHostedService), binding, "").Behaviors.Add(new WebHttpBehavior());
                foreach (ServiceEndpoint EP in localHost.Description.Endpoints)
                {
                    EP.Behaviors.Add(new BehaviorAttribute());
                }
                //Open
                localHost.Open();
            }
            catch (Exception ex)
            {
                LogWriter.Error("Failed to OpenWcfHost: " + ex);
            }
        }

        private static void CreateShortcut()
        {
            LogWriter.Info("CreateShortcut: ");

            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Spareio\SpareioApp";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 5)
            {
                startMenuPath = @"C:\Documents and Settings\All Users\Start Menu\Programs\Spareio\SpareioApp";
            }
            try
            {
                if (!System.IO.Directory.Exists(startMenuPath))
                    System.IO.Directory.CreateDirectory(startMenuPath);
                string shortcutLocation = Path.Combine(startMenuPath, executableLnk);
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
                shortcut.Arguments = "";
                shortcut.Description = "Sapreio App";
                //shortcut.IconLocation = LavasoftPathProvider.GetPathToIcon(); //uncomment to set the icon of the shortcut
                shortcut.TargetPath = XRewardPathConstants.GetPathToExe();
                shortcut.Save();
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex.ToString());
            }
        }

        public static void RunOnStartUp(bool command, string appName = "Spareio")
        {
            LogWriter.Info("RunOnStartUp: ");

            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(AppRegistryConstants.ApplicationStartupKey, true))
            {
                if (registryKey != null)
                {
                    // Set or remove the startup app in the registry.
                    if (command)
                    {
                        registryKey.SetValue(appName, String.Join(" ", new[]
                        {
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Spareio.exe"
                        }));
                    }
                    else
                    {
                        if (registryKey.GetValue(appName) != null)
                            registryKey.DeleteValue(appName);
                    }
                    // Save the setting in Settings.
                   
                }
            }
        }

        void CheckForUpdatesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LogWriter.Info("Update Timer Elapsed");
            _checkForSoftwareUpdateTimer.Interval = (int)(AppConstant.SoftwareUpdateInterval).TotalMilliseconds;
            _checkForSoftwareUpdateTimer.Start();
            CheckForUpdates();
        }


        private static void CheckForUpdates()
        {
            try
            {
                LogWriter.Info("Checking for update");
                if (InternetConnectionChecker.IsConnectedToInternet())
                {
                    string token = RegistryHelper.GetValue("xToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        VersionInfo info = EventService.GetVersion(token);
                        string expectedversion = info.expectedVersion;
                        LogWriter.Info("Live Version "+expectedversion);
                        Version liveVersion = new Version(expectedversion);
                        Version currentVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly());
                        bool isUpdateAvailable = currentVersion.CompareTo(liveVersion) == -1;
                        LogWriter.Info("IsUpdateAvailable Flag "+isUpdateAvailable);
                        if (isUpdateAvailable)
                            DoUpdate(expectedversion);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while checking for update "+ex.Message);
            }

        }

        private static void DoUpdate(string version)
        {
            LogWriter.Info("UpdateTime");
            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string nanoInstallerPath = Path.Combine(executablePath, "SpareioInstaller.exe");
            if (System.IO.File.Exists(nanoInstallerPath))
            {
                var nanoArgs = new StringBuilder();
                nanoArgs.AppendFormat(" --update --silent");
                nanoArgs.AppendFormat(" --version={0}", version);
                string token = RegistryHelper.GetValue("xToken");
                if (!String.IsNullOrEmpty(token))
                {
                    nanoArgs.AppendFormat(" --xToken={0}", token);
                }
                string partner = RegistryHelper.GetValue("PartnerId");
                {
                    nanoArgs.AppendFormat(" --partner={0}", partner);
                }
                string campaign = RegistryHelper.GetValue("CampaignId");
                {
                    nanoArgs.AppendFormat(" --campaign={0}", campaign);
                }

                LogWriter.Info("Update "+nanoInstallerPath +" with " +nanoArgs.ToString());

                SpareioClient.SpareioWCFClient client = new SpareioWCFClient();

                Process.Start(nanoInstallerPath, nanoArgs.ToString());
                //Call to Webservice


            }

        }


        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Monitoring win = new Monitoring();
            win.Show();
        }
    }

}
