using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using CoreLib;
using Spareio.WinService.Helper;
using System.ServiceModel;
using Spareio.WinService.DB;

namespace Spareio.WinService
{
    public partial class XRewardService : ServiceBase
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(XRewardService));
        private System.Timers.Timer _hourlyTimer;
        ServiceHost myHost;


        public XRewardService()
        {
            InitializeComponent();
            this.CanHandleSessionChangeEvent = true;
            this.CanHandlePowerEvent = true;
            this.CanShutdown = true;
        }

        public void OnDebug()
        {
            OnStart(new string[] { "0" });
        }
        protected override void OnStart(string[] args)
        {
            //Refactor: Add debug condition for testing purpose

            //EnableLogging();
            EnableTimer();

            //Refactor: Change XML to LiteDB : Location - C:\ProgramData\Spareio\Config
            DBHelper._currentRewardId = DBHelper.Initialize(DateTime.Now.ToString());

            //XmlHelper.LoadInMemory();

            //Refactor : Change logging to log4Net
            _logWriter.Info("Spareio winservice started");

            //Adding wcf host urlacl
            new Thread(AddUrlAcl).Start();
            bool isLoggedIn = false;
            //on first start after install, it will get token from installer which needs to be stored
            if (args.Length > 0)
            {
                isLoggedIn = args.Length > 0;
                DBHelper.Update(VariableConstants.xToken, args[0]);
            }
            else
                _logWriter.Info("Service started without token");

            //Monitoring start
            MonitorService.Initialize(isLoggedIn);

            OpenWCFHost();
        }

        private void OpenWCFHost()
        {
            _logWriter.Info("Opening wcf host Process ");
            try
            {
                myHost = new ServiceHost(typeof(Spareio.WCF.SpareioWCF));
                Uri address = new Uri("http://localhost:7097/SpareioWCF");
                WSHttpBinding binding = new WSHttpBinding();
                Type contract = typeof(Spareio.WCF.ISpareioWCF);
                myHost.AddServiceEndpoint(contract, binding, address);
                myHost.Open();
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while opening host " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            _logWriter.Info("Time to send hourly event with proper trigger");
            DBHelper.Update(VariableConstants.ServiceStopTime, DateTime.Now.ToString());
            MonitorService.Stop("close");
            if (myHost != null)
                myHost.Close();
        }

        protected override void OnShutdown()
        {
            _logWriter.Info("Shutting down");
            MonitorService.Stop("ShutDown");
            if (myHost != null)
                myHost.Close();
            base.OnShutdown();
        }

        private void EnableLogging()
        {
            try
            {
                //_logWriter.Enable = true;
                //_logWriter.Initialize("XRewardService");
            }
            catch (Exception ex)
            {
            }
        }

        private void EnableTimer()
        {
            _hourlyTimer = new System.Timers.Timer();
            _hourlyTimer.Interval = 3600000;
            _hourlyTimer.Elapsed += HourlyTimerOnElapsed;
            _hourlyTimer.AutoReset = true;
            _hourlyTimer.Start();
        }

        private void HourlyTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logWriter.Info("Timer elapsed.. time to send event");
            MonitorService.Stop("Interval");

            bool isLoggedIn = true;
            DBHelper._currentRewardId = 0;

            Boolean.TryParse(DBHelper.GetValById(VariableConstants.IsLoggedIn), out isLoggedIn);
            MonitorService.Initialize(isLoggedIn);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            _logWriter.Info("Session switched");
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLock:
                    _logWriter.Info(string.Format("Locked at {0}", DateTime.Now));
                    SessionService.HandleLogOff();
                    break;
                case SessionChangeReason.SessionLogoff:
                    _logWriter.Info(string.Format("Logged Off at {0}", DateTime.Now));
                    SessionService.HandleLogOff();
                    break;
                case SessionChangeReason.SessionLogon:
                    _logWriter.Info(string.Format("Logged On at {0}", DateTime.Now));
                    SessionService.HandleLogIn();
                    break;
                case SessionChangeReason.SessionUnlock:
                    _logWriter.Info(string.Format("Unlocked at {0}", DateTime.Now));
                    SessionService.HandleLogIn();
                    break;
                default:
                    break;

            }
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            _logWriter.Info("Power Mode change detected");
            PowerStatus status = SystemInformation.PowerStatus;
            if (powerStatus == PowerBroadcastStatus.PowerStatusChange)
            {
                if (status.PowerLineStatus == PowerLineStatus.Online)
                    PowerService.HandlePlugIn();
                if (status.PowerLineStatus == PowerLineStatus.Offline)
                    PowerService.HandlePlugOut();
            }

            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeSuspend))
            {
                _logWriter.Info("Resume suspended coming back from sleep");
            }

            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeAutomatic))
            {
                _logWriter.Info("Resume suspended coming back from sleep automatic");
                MonitorService.Initialize();
            }

            else if (powerStatus.HasFlag(PowerBroadcastStatus.QuerySuspend))
            {
                MonitorService.Stop("Sleep");

                _logWriter.Info("Going in sleeping mode");
                _logWriter.Info("Query suspended going to sleep");
                //MonitorService.Stop("Sleep");
            }

            return false;
        }

        public static void AddUrlAcl()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C netsh http add urlacl url=http://+:9007/ user=Everyone";
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                _logWriter.Info("Error while adding url " + ex.Message);
            }
        }

    }
}
