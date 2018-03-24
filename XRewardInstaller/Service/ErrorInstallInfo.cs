﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Service
{
    class ErrorInstallInfo
    {
        public string ErrorMessage { get; set; }
        public string batteryStatus { get; set; }
        public bool batteryUsed { get; set; }
        public bool isAdmin { get; set; }
        public string osVersion { get; set; }
        public List<string> screenRes { get; set; }

        public string partnerId { get; set; }
        public string campaignId { get; set; }
        public string machineId { get; set; }
        public string token { get; set; }
        public string sdkVersion { get; set; }
        public string timeZone { get; set; }
    }
}
