// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Base.Network.Events
{
    public class NetworkBaseErrorEventArgs : EventArgs
    {
        public NetworkBaseErrorEventArgs(string msg, Exception ex)
        {
            Msg = msg;
            Ex = ex;
        }

        public string Msg { get; }

        public Exception Ex { get; }
    }
}