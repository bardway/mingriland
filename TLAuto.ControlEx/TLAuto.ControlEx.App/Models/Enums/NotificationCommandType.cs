// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.ControlEx.App.Models.Enums
{
    public enum NotificationCommandType
    {
        [Description("状态检查")]
        Status,

        [Description("启动APP")]
        StartApp,

        [Description("关闭APP")]
        StopApp,

        [Description("关机")]
        ShutDown
    }
}