// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
#endregion

namespace TLAuto.Wcf.Server
{
    public class WsHttpWcfServerService : WcfServerServiceBase
    {
        private readonly TimeSpan _receiveTimeout;

        public WsHttpWcfServerService(TimeSpan receiveTimeout)
        {
            _receiveTimeout = receiveTimeout;
        }

        public WsHttpWcfServerService() :
            this(TimeSpan.FromMinutes(10)) { }

        protected override Binding GetServiceEndpointBindingType()
        {
            return new WSHttpBinding(SecurityMode.None) {ReceiveTimeout = _receiveTimeout};
        }

        protected override void AddServiceBehaviors(ServiceHost serviceHost)
        {
            if (serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            {
                var behavior = new ServiceMetadataBehavior
                               {
                                   HttpGetEnabled = true,
                                   HttpGetUrl = new Uri(ServiceAddress)
                               };
                serviceHost.Description.Behaviors.Add(behavior);
            }
            base.AddServiceBehaviors(serviceHost);
        }
    }
}