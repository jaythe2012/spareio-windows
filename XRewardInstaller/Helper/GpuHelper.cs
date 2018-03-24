using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Documents;
using System.Windows.Forms;
using Spareio.Installer.Service;

namespace Spareio.Installer.Helper
{
    class GpuHelper
    {
        public static List<GpuInfo> GetGpuInfo()
        {
            var gpuInfos = new List<GpuInfo>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
                string graphicsCard = string.Empty;
                foreach (ManagementObject obj in searcher.Get())
                {
                    var gpuInfo = new GpuInfo();
                    //foreach (PropertyData property in mo.Properties)
                    //{
                    //    if (property.Name == "Description")
                    //    {
                    //        graphicsCard = property.Value.ToString();
                    //    }
                    //    else
                    //    {
                    //        if (property.Value != null)
                    //        {
                    //            //displayConfiguration.Add(property.Name, property.Value);
                    //        }
                    //    }
                    //}
                    gpuInfo.bitsPerPel = obj.TryGetProperty("bitsPerPel").ToString();
                    gpuInfo.caption = obj.TryGetProperty("caption").ToString();
                    gpuInfo.deviceName = obj.TryGetProperty("deviceName").ToString();
                    gpuInfo.displayFlags = obj["displayFlags"].ToString();
                    gpuInfo.displayFrequency = obj["displayFrequency"].ToString();
                    gpuInfo.logPixels = obj.TryGetProperty("logPixels").ToString();
                    gpuInfo.pelsHeight = obj.TryGetProperty("pelsHeight").ToString();
                    gpuInfo.pelsWidth = obj.TryGetProperty("pelsWidth").ToString();
                    gpuInfo.settingID = obj.TryGetProperty("settingID").ToString();
                    gpuInfo.specificationVersion = obj.TryGetProperty("specificationVersion").ToString();
                    gpuInfos.Add(gpuInfo);
                }
                return gpuInfos;
            }
            catch (System.Exception ex)
            {
                return gpuInfos;
            }


        }

        public static List<string> GetScreenResolution()
        {
            List<string> screenList = new List<string>();
           
            foreach (Screen screen in Screen.AllScreens)
            {
                //string str = String.Format("Monitor {0}: {1} x {2} @ {3},{4}\n", monId, screen.Bounds.Width,
                //    screen.Bounds.Height, screen.Bounds.X, screen.Bounds.Y);
                string str = String.Format("{0}x{1} ", screen.Bounds.Width,
                    screen.Bounds.Height);
               screenList.Add(str);
            }
            return screenList;
        }
    }
}
