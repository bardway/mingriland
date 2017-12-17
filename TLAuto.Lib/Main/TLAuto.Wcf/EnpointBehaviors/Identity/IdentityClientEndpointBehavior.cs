// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
#endregion

namespace TLAuto.Wcf.EnpointBehaviors.Identity
{
    public abstract class IdentityClientEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // 植入“偷听器”客户端  
            clientRuntime.ClientMessageInspectors.Add(GetClientMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint endpoint) { }

        protected abstract IdentityClientMessageInspector GetClientMessageInspector();
    }
}