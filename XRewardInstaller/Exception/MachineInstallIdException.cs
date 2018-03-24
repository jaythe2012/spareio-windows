using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    class MachineInstallIdException : GenericException
    {
        internal MachineInstallIdException()
        {
        }

        internal MachineInstallIdException(System.Exception ex) : base(ex)
        {
        }
    }
}
