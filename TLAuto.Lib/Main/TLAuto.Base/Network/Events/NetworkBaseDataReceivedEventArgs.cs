// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Base.Network.Events
{
    public class NetworkBaseDataReceivedEventArgs : EventArgs
    {
        public NetworkBaseDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}