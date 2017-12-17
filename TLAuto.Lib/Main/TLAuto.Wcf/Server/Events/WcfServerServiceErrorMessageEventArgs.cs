// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Wcf.Server.Events
{
    public class WcfServerServiceErrorMessageEventArgs : WcfServerServiceMessageEventArgs
    {
        public WcfServerServiceErrorMessageEventArgs(string msg, Exception ex)
            : base(msg)
        {
            Ex = ex;
        }

        public Exception Ex { get; }
    }
}