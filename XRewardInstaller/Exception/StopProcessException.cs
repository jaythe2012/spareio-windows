using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    class StopProcessException : GenericException
    {
        internal StopProcessException()
        {
        }

        internal StopProcessException(System.Exception ex) : base(ex)
        {
        }
    }
}