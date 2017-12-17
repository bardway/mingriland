// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.PLC.ServiceData
{
    [Serializable]
    public class RelayItem
    {
        public RelayItem(int relayNumber, RelayStatus relayStatus)
        {
            RelayNumber = relayNumber;
            RelayStatus = relayStatus;
        }

        public int RelayNumber { get; }

        public RelayStatus RelayStatus { get; }
    }
}