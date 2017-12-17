// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.Device.PLC.ServiceData
{
    public enum SwitchStatus
    {
        [Description("常开")]
        NO = 0,

        [Description("常闭")]
        NC = 1
    }
}