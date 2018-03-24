namespace Spareio.Installer.Helper
{
    internal static class ExtensionHelper
    {
        public static object TryGetProperty(this System.Management.ManagementObject wmiObj, string propertyName)
        {
            object retval;
            try
            {
                retval = wmiObj.GetPropertyValue(propertyName);
            }
            catch (System.Management.ManagementException ex)
            {
                retval = "";
            }
            return retval;
        }
    }
}
