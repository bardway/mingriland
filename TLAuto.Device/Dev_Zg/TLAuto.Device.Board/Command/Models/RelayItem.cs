/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC.Command.Models
{
    public class RelayItem
    {
        public RelayItem(int relayNumber, RelayStatus relayStatus)
        {
            RelayNumber = relayNumber;
            RelayStatus = relayStatus;
        }

        public int RelayNumber { private set; get; }

        public RelayStatus RelayStatus { private set; get; }
    }
}
