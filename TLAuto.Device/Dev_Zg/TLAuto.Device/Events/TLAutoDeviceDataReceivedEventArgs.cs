// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Events
{
    public class TLAutoDeviceDataReceivedEventArgs : EventArgs
    {
        public TLAutoDeviceDataReceivedEventArgs(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; }
    }
}