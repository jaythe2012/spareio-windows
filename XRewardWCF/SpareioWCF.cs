using System;
using System.Diagnostics;

namespace Spareio.WCF
{
    public class SpareioWCF : ISpareioWCF
    {
        public int RunProcess(string fullPath, string args)
        {
            try
            {
                return Process.Start(fullPath, args).Id;
            }
            catch (Exception ex)
            {
            }

            return -1;
        }
    }
}
