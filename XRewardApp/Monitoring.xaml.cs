using CoreLib;
using Spareio.Business;
using Spareio.UI.WcfHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Spareio.UI
{
    /// <summary>
    /// Interaction logic for Monitoring.xaml
    /// </summary>
    public partial class Monitoring : Window
    {
        private System.Timers.Timer mTimer;

        public Monitoring()
        {
            InitializeComponent();
            lblChangeStatus.Content = "OFF";

            btnTurnOn_Init.Visibility = Visibility.Hidden;
            btnTurnOn.Visibility = Visibility.Hidden;
            btnSnooze.Visibility = Visibility.Hidden;


            var mineConfig = GetMineConfig();


            //if (FirstTimeApp)
            //{
            //    btnTurnOn_Init.Visibility = Visibility.Visible;
            //}

            if (mineConfig == null) //If no entry in DB, just need to insert values App = On, Mine = false, Turn ON Button will be initialize
            {
                InitMineConfig();
                btnTurnOn.Visibility = Visibility.Visible;
            }

            if (mineConfig.Item1 == false) // Is App Off
            {
                UpdateMineConfig(true, null);
            }

            if (mineConfig.Item2 == true) // Is Mining On
            {
                btnSnooze.Visibility = Visibility.Visible;
            }
            else if (mineConfig.Item2 == false) // Is Mining Off
            {
                btnTurnOn.Visibility = Visibility.Visible;
            }

        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateMineConfig(false, null);

                foreach (var process in Process.GetProcessesByName("SpareioWinService"))
                {
                    process.Kill();
                }

            }
            catch (Exception ex)
            {
                LogWriter.Error("Error on stopping miner " + ex.Message);
            }

            this.Close();

        }

        private void mTimer_Tick(object sender, EventArgs e)
        {
            UpdateMineConfig(true, true);

            mTimer.Enabled = false;
            mTimer.Stop();
        }

        private void btnTurnOn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblChangeStatus.Content = "ON";
                imgChangeStatus.Source = new BitmapImage(new Uri("~/Assets/Images/Power_On.png"));

                btnSnooze.Visibility = Visibility.Visible;
                btnTurnOn_Init.Visibility = Visibility.Hidden;
                btnTurnOn.Visibility = Visibility.Hidden;

                UpdateMineConfig(true, true);
            }
            catch (Exception ex)
            {

            }


        }

        private void InitMineConfig()
        {
            MineConfigBL.Initialize(new Spareio.Model.MineConfigModel {
                IsAppOn = true,
                IsMiningOn = false
            });
        }
        private void UpdateMineConfig(bool? isAppOn, bool? isMiningOn)
        {
            MineConfigBL.Update(isAppOn, isMiningOn);
        }

        private Tuple<bool, bool> GetMineConfig()
        {
            var mineConfig = MineConfigBL.Get();

            if (mineConfig != null)
                // returnVal.Add(mineConfig.IsAppOn, mineConfig.IsMiningOn);
                return new Tuple<bool, bool>(mineConfig.IsAppOn, mineConfig.IsMiningOn);
            else
                return null;
        }

        private void EnableTimer()
        {
            LogWriter.Info("Enabling Timer");
            ServiceController.Instance.CleanUpData();
            mTimer = new System.Timers.Timer();
            mTimer.Interval = 7200000; // 2 Hours
            mTimer.Elapsed += mTimer_Tick;
            mTimer.AutoReset = true;
            mTimer.Start();
        }

        private void btnSnooze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblChangeStatus.Content = "SNOOZED";
                imgChangeStatus.Source = new BitmapImage(new Uri("~/Assets/Images/Power_Snooze.png"));

                btnTurnOn.Visibility = Visibility.Visible;
                btnTurnOn_Init.Visibility = Visibility.Hidden;
                btnSnooze.Visibility = Visibility.Hidden;

                UpdateMineConfig(true, false);
                EnableTimer();
            }
            catch (Exception ex)
            {

            }
        }
        private void btnTurnOn_Init_Click(object sender, RoutedEventArgs e)
        {
            UpdateMineConfig(true, true);
        }
    }
}
