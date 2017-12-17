// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endregion

namespace TLAuto.Wcf.Client
{
    public class WsHttpWcfClientService<TProxy> : WcfClientServiceBase<TProxy>
    {
        public WsHttpWcfClientService(string serviceAddress, TimeSpan sendTimeout) :
            base(serviceAddress, sendTimeout) { }

        public WsHttpWcfClientService(string serviceAddress) :
            this(serviceAddress, TimeSpan.FromMinutes(1)) { }

        protected override Binding GetServiceEndpointBindingType()
        {
            return new WSHttpBinding(SecurityMode.None) {MaxReceivedMessageSize = 2147483647};
        }
    }
}