namespace Spareio.Installer.Exception
{
    class DownloadException : GenericException
    {
        internal DownloadException()
        {
        }

        internal DownloadException(System.Exception ex) : base(ex)
        {
        }
    }
}
