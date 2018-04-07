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
                btnTurnOn.Visibility = Visibility.Hidden;
                btnSnooze.Visibility = Visibility.Visible;
                UpdateMineConfig(true, true);
            }
            catch (Exception ex)
            {

            }


        }
        private void UpdateMineConfig(bool? isAppOn, bool? isMiningOn)
        {
            MineConfigBL.Update(isAppOn, isMiningOn);
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
                btnTurnOn.Visibility = Visibility.Hidden;
                btnSnooze.Visibility = Visibility.Visible;
                UpdateMineConfig(true, false);
                EnableTimer();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
