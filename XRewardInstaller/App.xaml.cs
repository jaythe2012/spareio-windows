using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Spareio.Installer.Utils;

namespace Spareio.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string ErrorMessage = String.Empty;

        [STAThread]
        public static int Main()
        {
            var cmdArgs = new CmdLineArgs(Environment.GetCommandLineArgs());


            Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(Path.GetTempPath(), "XrInstaller.log")));
            Trace.AutoFlush = true;


            // note partner and campaign for further reporting
            //var cmdline = String.Format("Starting installer {0} with: {1}, Run as admin: {2}", CommunicationUtils.DataCollector.Version, String.Join(" ", Environment.GetCommandLineArgs()), IsAdministrator());


            if (!cmdArgs.CheckArg("uninstall"))
            {
                try
                {
                    var random = new Random((int)DateTime.Now.Ticks);
                    // no need to update installer for uninstall. It knows how to uninstall the current version
                    var tmpFolder = Path.Combine(Path.GetTempPath(), "wctmp_" + random.Next());
                }
                catch (System.Exception ex)
                {
                    var msg = "Internet connectivity problem.";
                    if (cmdArgs.CheckArg("silent"))
                    {
                        Console.WriteLine(msg);
                        return -1;
                    }

                    ErrorMessage = msg;

                }
            }

            // run scenario detection to override install by update
            if (cmdArgs.CheckArg("silent") || cmdArgs.CheckArg("update"))
            {
                // run installation without gui
                if (cmdArgs.CheckArg("uninstall"))
                {
                    Controller.RunUninstall(() => false, Environment.GetCommandLineArgs());
                    MessageBox.Show("UnInstallation Complete");

                }
                else if (cmdArgs.CheckArg("update"))
                {
                    
                    Controller.RunUpdate(() => false, Environment.GetCommandLineArgs());
                }
                else
                {
                    //InstallerProcesses();
                    Controller.RunInstallation(() => false,  Environment.GetCommandLineArgs());
                    //MessageBox.Show("Installation Complete");
                }
            }
            else
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }

            return 0;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var cmdArgs = new CmdLineArgs(Environment.GetCommandLineArgs());

            //Check if an instance is already running. If it is the case, warn the user and don't start the program.
            if ((System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) && !cmdArgs.CheckArg("uninstall"))
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5)); // retry in case of restart with update installer version
                if ((System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) && !cmdArgs.CheckArg("uninstall"))
                {
                    //We don't need a pop-up twice or in silent mode. Just close it then.
                    if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 2 || cmdArgs.CheckArg("silent"))
                    {
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        string originalFileName = string.Empty;
                        Process[] processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));

                        foreach (Process process in processes)
                        {
                            if (process.Modules[0].FileVersionInfo.OriginalFilename == System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location))
                            {
                                //We don't need a pop-up twice or in silent mode. Just close it then.
                                MainWindow mainView = new MainWindow();
                                //var page = new MultipleInstanceError();
                                //mainView.Content = page;
                                mainView.Show();
                                break;
                            }
                        }

                    }
                }
            }
            else
            {

                if (String.IsNullOrEmpty(ErrorMessage))
                {
                    if (cmdArgs.CheckArg("uninstall"))
                    {
                        //UninstallMainWindow unMainWin = new UninstallMainWindow();
                        //unMainWin.Show();
                    }
                    else
                    {
                        MainWindow mainView = new MainWindow();
                        mainView.Show();
                    }
                }
                else
                {
                    if (!cmdArgs.CheckArg("uninstall"))
                    {
                        MainWindow mainView = new MainWindow();
                        //var page = new CustomErrorView(ErrorType.InternetError);
                        //mainView.Content = page;
                        mainView.Show();
                    }
                }
            }
        }
    }
}
