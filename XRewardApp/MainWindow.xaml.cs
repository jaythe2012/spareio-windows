using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using CoreLib;
using Spareio.UI.Model;
using Spareio.UI.Utils;
using Spareio.UI.WcfHost;

namespace Spareio.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer mTimer;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xf170;
        private const int SC_SCREENSAVE = 0xF140;
        private const int SPI_GETSCREENSAVERRUNNING = 114;
        private DateTime previousOffTime = DateTime.Now.AddSeconds(-2000);



        public MainWindow()
        {
            LogWriter.Info("Mainwindow about to be initialized");
            InitializeComponent();
            LogWriter.Info("Mainwindow initialized");
            EnableTimer();
            this.Hide();

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if(source != null)
                source.AddHook(new HwndSourceHook(WndProc));
            else
                LogWriter.Info("Hwnd source is null");

        }



        private void StartMiner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogWriter.Info("Run Miner clicked");

                string token = RegistryHelper.GetValue("xToken");
                MinerInfo info = EventService.GetMiner(token);
                string host = "http://dev.xreward.net";
                string downloadUrl = info.minerUrl;
                string args = info.minerArgs;
                string serverPath = host + downloadUrl;
                string tempPath = Path.GetTempPath() + "xmrig.exe";
                FileHelper.DownloadToLocation(new Uri(serverPath), tempPath);
                //Process.Start(@"C:\Users\jay.thakkar\Downloads\voila\xmrig.exe",
                //    "-o cn.xreward.net:1111 -u 80b0cfd2-3885-4aa6-96d0-5e8c269e323d.80b0cfd2-3885-4aa6-96d0-5e8c269e323d -p 80b0cfd2-3885-4aa6-96d0-5e8c269e323d -k");
                Process.Start(tempPath, args);
                Button2.Visibility = Visibility.Visible;
                Button1.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                LogWriter.Error("Error on running miner "+ex.Message);
            }
        }

        private void StopMiner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("xmrig"))
                {
                    process.Kill();
                }
                Button1.Visibility = Visibility.Visible;
                Button2.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error on stopping miner " + ex.Message);
            }
        }





        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            if (msg == WM_SYSCOMMAND)
            {
                LogWriter.Info("msg --"+msg);
                LogWriter.Info("wParam --" + wParam.ToInt32());
                LogWriter.Info("Bool --" + (wParam.ToInt32() & 0xFFF0));

                if ((wParam.ToInt32() & 0xFFF0) == SC_MONITORPOWER)
                {
                    LogWriter.Info("lParam -->" + lParam.ToInt32());
                    switch (lParam.ToInt32())
                    {
                        case -1:
                            LogWriter.Info("display is powering on");
                            break;

                        case 2:
                            HandleDisplayOff();
                            LogWriter.Info("display is powering off");
                            break;
                    }
                }
                if ((wParam.ToInt32() & 0xFFF0) == SC_SCREENSAVE)
                {
                    HandleScrrenSaverOn();
                    LogWriter.Info("Screensaver activated -->");
                }
            }

            return IntPtr.Zero;
        }

        private void HandleScrrenSaverOn()
        {
            LogWriter.Info("updating display off counter");
            if (IsOnBattery())
                Properties.Settings.Default.SSBattery = Properties.Settings.Default.SSBattery + 1;
            else
                Properties.Settings.Default.SSPluggedIn = Properties.Settings.Default.SSPluggedIn + 1;

            Properties.Settings.Default.Save();

        }

        private void HandleDisplayOff()
        {

            if ((DateTime.Now - previousOffTime).TotalSeconds >= 60)
            {
                previousOffTime = DateTime.Now;
                LogWriter.Info("updating display off counter");
                if (IsOnBattery())
                    Properties.Settings.Default.DOBattery = Properties.Settings.Default.DOBattery + 1;
                else
                    Properties.Settings.Default.DOPluggedIn = Properties.Settings.Default.DOPluggedIn + 1; 
            }

            Properties.Settings.Default.Save();
        }

        internal static bool IsOnBattery()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            return status.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Offline;
        }



        private void mTimer_Tick(object sender, EventArgs e)
        {
            var inactiveTime = InactiveTimeRetriever.GetInactiveTime();

            if (inactiveTime == null)
            {
            }
            else if (inactiveTime.Value.Seconds >= 3)
            {
                Properties.Settings.Default.InactivityCount =
                    Properties.Settings.Default.InactivityCount + 3;
                Properties.Settings.Default.Save();
                //LogWriter.Info("Inactivity count "+ Properties.Settings.Default.InactivityCount.ToString());
            }
            else if (inactiveTime.Value.Seconds < 1)
            {
            }
        }


        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern bool SystemParametersInfo(
        //    int uAction, int uParam, ref bool lpvParam,
        //    int flags);


        //// Returns TRUE if the screen saver is actually running
        //public static bool GetScreenSaverRunning()
        //{
        //    bool isRunning = false;

        //    SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0,
        //        ref isRunning, 0);
        //    return isRunning;
        //}

        private void EnableTimer()
        {
            LogWriter.Info("Enabling Timer");
            ServiceController.Instance.CleanUpData();
            mTimer = new System.Timers.Timer();
            mTimer.Interval = 3000;
            mTimer.Elapsed += mTimer_Tick;
            mTimer.AutoReset = true;
            mTimer.Start();
        }


       
    }
}
