// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Configuration;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.ControlEx.App.Common
{
    public static class ConfigHelper
    {
        public static string PLCServiceKey => ConfigurationManager.AppSettings["PLCServiceKey"];

        public static string ProjectorServiceKey => ConfigurationManager.AppSettings["ProjectorServiceKey"];

        public static string DMXServiceKey => ConfigurationManager.AppSettings["DMXServiceKey"];

        public static int QuerySwitchTime => ConfigurationManager.AppSettings["QuerySwitchTime"].ToInt32();

        public static string StartUpFilePath { set; get; }
    }
}