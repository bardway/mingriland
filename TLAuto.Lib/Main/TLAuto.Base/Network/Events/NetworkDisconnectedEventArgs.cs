// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Base.Network.Events
{
    public class NetworkDisconnectedEventArgs : EventArgs
    {
        public NetworkDisconnectedEventArgs(string msg, Exception ex)
        {
            Msg = msg;
            Ex = ex;
        }

        public string Msg { get; }

        public Exception Ex { get; }
    }
}