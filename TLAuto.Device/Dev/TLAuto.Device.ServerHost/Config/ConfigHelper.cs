// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Configuration;

using TLAuto.Base.Extensions;
using TLAuto.Device.ServerHost.Properties;
#endregion

namespace TLAuto.Device.ServerHost.Config
{
    public static class ConfigHelper
    {
        public static string WcfServiceAddress => Settings.Default.WcfServiceAddress;

        public static bool IsAutoStartWcf => Settings.Default.IsAutoStartWcf;

        public static int SerialPortSendTimeInterval => ConfigurationManager.AppSettings[nameof(SerialPortSendTimeInterval)].ToInt32();

        public static void Init()
        {
            if (WcfServiceAddress.IndexOf("Localhost", StringComparison.Ordinal) != -1)
            {
                var ips = IpExtensions.GetInternetIpsWithoutVirtual();
                if (ips.Count >= 1)
                {
                    Settings.Default.WcfServiceAddress = WcfServiceAddress.Replace("Localhost", ips[0]);
                }
            }
        }

        public static void SaveConfig(string wcfServiceAddress, bool isAutoStartWcf)
        {
            Settings.Default.WcfServiceAddress = wcfServiceAddress;
            Settings.Default.IsAutoStartWcf = isAutoStartWcf;
            Settings.Default.Save();
            Settings.Default.Reload();
        }
    }
}