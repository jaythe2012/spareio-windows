using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    internal class ExecuteCommandException : GenericException
    {
        internal ExecuteCommandException()
        {
        }

        internal ExecuteCommandException(System.Exception ex)
            : base(ex)
        {
        }
    }
}
