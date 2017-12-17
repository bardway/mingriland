// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Wcf.Client.Events
{
    public class WcfClientServiceErrorEventArgs : EventArgs
    {
        public WcfClientServiceErrorEventArgs(string msg, Exception ex)
        {
            Msg = msg;
            Ex = ex;
        }

        public string Msg { get; }

        public Exception Ex { get; }
    }
}