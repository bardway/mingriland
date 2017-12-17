// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
#endregion

namespace TLAuto.Wcf.EnpointBehaviors.Identity
{
    public abstract class IdentityServerMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (HasValidateAction(request.Headers.Action))
            {
                //栓查验证信息  
                AnalysisHeaders(request.Headers);
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        protected abstract bool HasValidateAction(string action);

        protected abstract void AnalysisHeaders(MessageHeaders messageHeaders);
    }
}