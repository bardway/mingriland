// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.Wcf.Client;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.ControlEx.App
{
    public class SendWcfCommand<T> : NetTcpWcfClientService<T>
    {
        private readonly Action<string> _writeLogMsgAction;

        public SendWcfCommand(string serviceAddress, Action<string> writeLogMsgAction, TimeSpan sendTimeout)
            : base(serviceAddress, sendTimeout)
        {
            _writeLogMsgAction = writeLogMsgAction;
        }

        public SendWcfCommand(string serviceAddress, Action<string> writeLogMsgAction)
            : this(serviceAddress, writeLogMsgAction, TimeSpan.FromMinutes(1)) { }

        protected override void OnError(WcfClientServiceErrorEventArgs e)
        {
            _writeLogMsgAction(e.Msg);
            base.OnError(e);
        }

        protected override void OnCommunicationError(WcfClientServiceErrorEventArgs e)
        {
            _writeLogMsgAction(e.Msg);
            base.OnCommunicationError(e);
        }

        protected override void OnTimeoutError(WcfClientServiceErrorEventArgs e)
        {
            _writeLogMsgAction(e.Msg);
            base.OnTimeoutError(e);
        }

        public new void Close()
        {
            base.Close();
        }
    }
}