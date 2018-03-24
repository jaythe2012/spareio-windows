using System;
using System.IO;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class RemoveFolder : IInstallationStep
    {
        protected string _path = null;
        public RemoveFolder(string path)
        {
            _path = path;
        }

        public void Report()
        {
            string folderName = string.Empty;

            try
            {
                if (Directory.Exists(_path))
                {
                    var di = new DirectoryInfo(_path);
                    folderName = di.Name;
                }
                else
                {
                    folderName = _path;
                }
            }
            catch (System.Exception ex)
            {
                folderName = _path;
            }

            CommunicationUtils.SendReport(String.Format("The following folder has been removed - {0}", folderName));
        }

        public virtual void Perform()
        {
            try
            {
                if (Directory.Exists(_path))
                {
                    var di = new DirectoryInfo(_path);

                    // remove the read only first due to the problem of deleteing the files/folders
                    foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                    {
                        file.Attributes &= ~FileAttributes.ReadOnly;
                    }

                    di.Delete(true);
                }
            }
            catch (System.Exception ex)
            {
                throw new RemoveFolderException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Removing folder {0} ...", _path), false);
        }
    }
}
