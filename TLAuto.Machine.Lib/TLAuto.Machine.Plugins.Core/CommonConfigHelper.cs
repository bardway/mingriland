// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Configuration;
using System.IO;
#endregion

namespace TLAuto.Machine.Plugins.Core
{
    public static class CommonConfigHelper
    {
        public static string PluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachinePlugins");

        public static string MusicBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachinePlugins", "Music");

        public static string VideoBasePath = Path.Combine(@"C:\Program Files\StartGateServer\TLAuto.Video\TLAuto.Video.App.exe");

        public static string PLCServiceKey => ConfigurationManager.AppSettings["PLCServiceKey"];

        public static string NotificationServiceAddress => ConfigurationManager.AppSettings[nameof(NotificationServiceAddress)];
    }
}