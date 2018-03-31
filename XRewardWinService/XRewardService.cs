using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Spareio.WinService.Helper;
using System.ServiceModel;
using Spareio.WinService.DB;

namespace Spareio.WinService
{
    public partial class XRewardService : ServiceBase
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(XRewardService));
        private System.Timers.Timer _minuteTimer;

        static string[] _args;
        static DateTime _startTimeOfService;


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

            //  var r = MineService.ReadyToMine();
        }

        protected override void OnStart(string[] args)
        {

            //Refactor: Add debug condition for testing purpose
            EnableTimer();

            //Refactor: Change XML to LiteDB : Location - C:\ProgramData\Spareio\Config
            //DBHelper.CurrentRewardId = DBHelper.Initialize(DateTime.Now.ToString());

            //Refactor : Change logging to log4Net
            _logWriter.Info("Spareio winservice started");

            //Adding wcf host urlacl
            new Thread(AddUrlAcl).Start();

            //Open WCF Host
            OpenWCFHost();


            if ((_args != null && _args.Length > 0) == false)
                _logWriter.Info("Service started without token");


            //Ping service every on service start
            _logWriter.Info("Ping Mine Server");
            MineService.PingMineServer();

            //Cpu Service Initialization
            CpuService.Initialize();


            _logWriter.Info("Init mine on startup.");
            if (InitMine())
            {
                MineService.mineCounter += 1;
            }

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

        private void EnableTimer()
        {
            _startTimeOfService = DateTime.Now;
            _minuteTimer = new System.Timers.Timer();
            _minuteTimer.Interval = 60000D;
            _minuteTimer.Elapsed += MinuteTimerOnElapsed;
            _minuteTimer.AutoReset = true;
            _minuteTimer.Start();

        }

        private void MinuteTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckEveryMinute();
        }

        private void CheckEveryMinute()
        {
            TimeSpan diff = DateTime.Now - _startTimeOfService;
            double minutes = diff.Minutes;

            _logWriter.Info("Minute Timer elapsed.. time to validate JSON");

            if (diff.Days == 1) // Next Day
            {
                //In any condition mine counter will be zero.
                MineService.mineCounter = 0;

                _logWriter.Info("Day elapsed.. time to send event");
                ResetMine();
            }
            else
            {
                if (minutes >= 1 && minutes <= 60)
                {
                    if (MineService.ReadyToMine())
                    {
                        if (MineService.mineCounter == 0) InitMine();
                        else
                        {
                            Mine();
                            MineService.mineCounter += 1;
                        }
                    }
                }

                if (minutes == 60)
                {
                    _logWriter.Info("Hour elapsed.. time to send event");
                    ResetMine();
                }
            }
        }

        private bool InitMine()
        {

            MineService.ReadyToMine();
            if (MineService.ReadyToMine())
            {
                DBHelper.CurrentRewardId = DBHelper.Initialize(DateTime.Now.ToString());
                bool isLoggedIn = false;

                //on first start after install, it will get token from installer which needs to be stored
                if (_args != null && _args.Length > 0)
                {
                    isLoggedIn = _args.Length > 0;
                    DBHelper.Update(VariableConstants.xToken, _args[0]);
                }

                //Monitoring start
                MonitorService.Initialize(isLoggedIn);

                return true;
            }
            else
                return false;

        }

        private void Mine()
        {
            if (MineService.ReadyToMine())
            {
                CpuService.UpdateCpuAverage();
            }

        }

        private void ResetMine()
        {
            bool isLoggedIn = true;
            var timeToWorkPerDay = (MineService.timeToWorkPerDay != 0 ? MineService.timeToWorkPerDay : (60 * 60)) / 60;


            SendTelemetry("Interval");

            if (MineService.mineCounter >= timeToWorkPerDay)
                MineService.mineCounter = 0;
            else
            {
                Boolean.TryParse(DBHelper.GetValById(VariableConstants.IsLoggedIn), out isLoggedIn);
                MonitorService.Initialize(isLoggedIn);

                DBHelper.CurrentRewardId = 0;
                _startTimeOfService = DateTime.Now;

                //Ping service every an hour
                MineService.PingMineServer();
            }
        }


        protected override void OnStop()
        {
            _logWriter.Info("Time to send hourly event with proper trigger");
            DBHelper.Update(VariableConstants.ServiceStopTime, DateTime.Now.ToString());

            SendTelemetry("Close");

            if (myHost != null)
                myHost.Close();
        }

        protected override void OnShutdown()
        {
            _logWriter.Info("Shutting down");
            DBHelper.Update(VariableConstants.ServiceStopTime, DateTime.Now.ToString()); //Added for Shutdown
            SendTelemetry("ShutDown");


            if (myHost != null)
                myHost.Close();
            base.OnShutdown();
        }

        private void SendTelemetry(string trigger)
        {
            _logWriter.Info("Send Telemetry - " + trigger);
            MonitorService.Stop(trigger);
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
                //  MonitorService.Stop("Sleep");
                SendTelemetry("Sleep");

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
