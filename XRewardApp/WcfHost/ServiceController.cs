using System;
using System.Collections.Generic;
using CoreLib;

namespace Spareio.UI.WcfHost
{
    class ServiceController
    {

        #region Fields
        private static volatile ServiceController instance;
        private static object sync = new Object();
        public static Dictionary<string, string> ResponseDictionary = new Dictionary<string, string>();
        #endregion


        public static ServiceController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (sync)
                    {
                        if (instance == null)
                        {
                            try
                            {
                                instance = new ServiceController();
                            }
                            catch (Exception ex)
                            {
                               LogWriter.Error(ex.Message);
                            }
                        }
                    }
                }
                return instance;
            }
        }

        internal string GetInfo()
        {
            string response = String.Empty;
            try
            {
                //Prepare Data
                ResponseDictionary = ReadData();
                response = ResponseObject.GetValue(ResponseDictionary);
                CleanUpData();
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while sending response" +ex.Message);
            }
            return response;
        }

        private Dictionary<string, string> ReadData()
        {
            Dictionary<string,string> tempDictionary = new Dictionary<string, string>();
            try
            {
                tempDictionary.Add("SSBattery", Properties.Settings.Default.SSBattery.ToString());
                tempDictionary.Add("SSPluggedIn", Properties.Settings.Default.SSPluggedIn.ToString());
                tempDictionary.Add("DOBattery", Properties.Settings.Default.DOBattery.ToString());
                tempDictionary.Add("DOPluggedIn", Properties.Settings.Default.DOPluggedIn.ToString());
                if(Properties.Settings.Default.InactivityCount.ToString() == "3417" || Properties.Settings.Default.InactivityCount.ToString() == "3431")
                    tempDictionary.Add("InactivityCount", "3600");
                else
                    tempDictionary.Add("InactivityCount", Properties.Settings.Default.InactivityCount.ToString());
            }
            catch (Exception ex)
            {
               LogWriter.Error("Error while reading properties " +ex.Message);
            }
            return tempDictionary;
        }

        internal void CleanUpData()
        {
            Properties.Settings.Default.SSBattery = 0;
            Properties.Settings.Default.SSPluggedIn = 0;
            Properties.Settings.Default.DOBattery = 0;
            Properties.Settings.Default.DOPluggedIn = 0;
            Properties.Settings.Default.InactivityCount = 0.0;
            Properties.Settings.Default.Save();


        }
    }
}
