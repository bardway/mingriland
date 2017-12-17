// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.Device.PLC.ServiceData
{
    public enum RelayStatus
    {
        [Description("常闭")]
        NC = 0,

        [Description("常开")]
        NO = 1
    }
}