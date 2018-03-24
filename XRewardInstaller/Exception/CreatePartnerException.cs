using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.Installer.Exception
{
    class CreatePartnerException : GenericException
    {
        internal CreatePartnerException()
        {
        }

        internal CreatePartnerException(System.Exception ex)
            : base(ex)
        {
        }
    }
}
