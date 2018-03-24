using System.IO;
using Spareio.Installer.Exception;

namespace Spareio.Installer.AppCore
{
    internal class RemoveAppdataFolder : RemoveFolder
    {
        protected string _startWith = null;
        public RemoveAppdataFolder(string path, string startWith) : base(path)
        {
            _startWith = startWith;
        }

        public override void Perform()
        {
            try
            {
                if (Directory.Exists(_path))
                {
                    foreach (var folder in Directory.GetDirectories(_path))
                    {
                        if (folder.Contains(_startWith))
                        {
                            if (Directory.Exists(folder))
                            {
                                Directory.Delete(folder, true);
                            }
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                throw new RemoveFolderException(ex) { Step = this };
            }
        }
    }
}
