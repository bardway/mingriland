// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.PLC.ServiceData
{
    [Serializable]
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