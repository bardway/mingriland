/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC.Command.Models
{
    public sealed class SwitchItem
    {
        public SwitchItem(int switchNumber, SwitchStatus switchStatus)
        {
            SwitchNumber = switchNumber;
            SwitchStatus = switchStatus;
        }

        public int SwitchNumber { get; }

        public SwitchStatus SwitchStatus { get; }
    }
}
