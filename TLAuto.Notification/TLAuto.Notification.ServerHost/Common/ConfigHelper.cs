// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Configuration;
#endregion

namespace TLAuto.Notification.ServerHost.Common
{
    public static class ConfigHelper
    {
        public static string ServiceAddress => ConfigurationManager.AppSettings["ServiceAddress"];

        public static void SaveConfig(string serviceAddress)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["ServiceAddress"].Value = serviceAddress;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}