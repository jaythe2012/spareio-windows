using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    class RemoveFolderException : GenericException
    {
        internal RemoveFolderException()
        {
        }

        internal RemoveFolderException(System.Exception ex)
            : base(ex)
        {
        }
    }
}