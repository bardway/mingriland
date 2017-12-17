/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System.ComponentModel;

namespace TLAuto.Device.PLC.Command.Models.Enums
{
    public enum RelayStatus
    {
        [Description("常闭")]
        NC = 0,
        [Description("常开")]
        NO = 1
    }
}
