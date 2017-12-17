// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.View.Models.Events
{
    public class SwitchItemChangedEventArgs : EventArgs
    {
        public SwitchItemChangedEventArgs(SwitchItem switchItem)
        {
            SwitchItem = switchItem;
        }

        public SwitchItem SwitchItem { get; }
    }
}