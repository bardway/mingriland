// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Configuration;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.PLC.View.Config
{
    public static class ConfigHelper
    {
        public static int SerialPortSendTime => ConfigurationManager.AppSettings["SerialPortSendTimeInterval"].ToInt32();
    }
}