namespace Spareio.Installer.AppCore
{
    internal interface IInstallationStep
    {
        void Report();
        void Perform();
        void Init();
    }
}
