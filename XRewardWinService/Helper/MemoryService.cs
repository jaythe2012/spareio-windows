using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.WinService.Helper
{
    public class MemoryService
    {
        public static string GetAvailableMemory()
        {
            var performance = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            return performance.NextValue().ToString();
        }

    }
}
