// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Wcf.Server.Events
{
    public class WcfServerServiceMessageEventArgs : EventArgs
    {
        public WcfServerServiceMessageEventArgs(string msg)
        {
            Msg = msg;
        }

        public string Msg { get; }
    }
}