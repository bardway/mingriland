// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Configuration;

using TLAuto.Machine.Controls.Models.Enums;
#endregion

namespace TLAuto.Machine.App.Common
{
    public static class ConfigHelper
    {
        static ConfigHelper()
        {
            DiffType = DifficulySystemType.Low;
        }

        public static string NotificationServiceAddress => ConfigurationManager.AppSettings[nameof(NotificationServiceAddress)];

        public static string TestMachineKey => ConfigurationManager.AppSettings[nameof(TestMachineKey)];

        public static DifficulySystemType DiffType { internal set; get; }

        public static string[] Args { set; get; }

        public static string Title => (Args != null) && (Args.Length >= 1) ? Args[0] : "机关";

        public static string MachineKey => (Args != null) && (Args.Length >= 2) ? Args[1] : TestMachineKey;
    }
}