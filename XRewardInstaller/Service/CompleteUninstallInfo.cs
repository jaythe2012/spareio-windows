using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Service
{
    class CompleteUninstallInfo
    {
        public bool isAdmin { get; set; }
        public string osVersion { get; set; }
        public string partnerId { get; set; }
        public string campaignId { get; set; }
        public string installDate { get; set; }
        public string machineId { get; set; }
        public string token { get; set; }
        public string miners { get; set; }
        public string HVVersion { get; set; }
    }
}
