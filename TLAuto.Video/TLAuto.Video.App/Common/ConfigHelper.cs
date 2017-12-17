// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Configuration;
#endregion

namespace TLAuto.Video.App.Common
{
    public static class ConfigHelper
    {
        public static string ServiceAddress => ConfigurationManager.AppSettings[nameof(ServiceAddress)];
    }
}