// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.Notification.Contracts
{
    public enum AppStatusType
    {
        [Description("无或异常")]
        None = 0,

        [Description("停止")]
        Stop = 1,

        [Description("启动")]
        Start = 2
    }
}