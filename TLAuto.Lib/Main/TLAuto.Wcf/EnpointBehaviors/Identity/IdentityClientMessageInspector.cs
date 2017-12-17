// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
#endregion

namespace TLAuto.Wcf.EnpointBehaviors.Identity
{
    public abstract class IdentityClientMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var messageHeaders = GetMessageHeaders();
            foreach (var messageHeader in messageHeaders)
            {
                request.Headers.Add(messageHeader);
            }
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        protected abstract IEnumerable<MessageHeader> GetMessageHeaders();
    }
}