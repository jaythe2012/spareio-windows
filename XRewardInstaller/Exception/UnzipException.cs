using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    internal class UnzipException : GenericException
    {
        internal UnzipException()
        {
        }

        internal UnzipException(System.Exception ex)
            : base(ex)
        {
        }
    }
}
