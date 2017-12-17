// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.ControlEx.App.Models.Enums
{
    public enum ExcuteType
    {
        [Description("开关检测执行器")]
        Switch,

        [Description("继电器控制执行器")]
        Relay,

        [Description("投影仪执行器")]
        Projector,

        [Description("延时执行器")]
        Delay,

        [Description("音乐执行器")]
        Music,

        [Description("应用通知执行器")]
        Notification,

        [Description("舞台灯光执行器")]
        DMX
    }
}