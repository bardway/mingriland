// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Machine.Plugins.HitCheckTask
{
    public sealed class SwitchItemWithDeviceNumber
    {
        public Guid Stamp { get; set; }

        public SwitchItem SwitchItem { get; set; }

        public int DeviceNumber { get; set; }
    }
}