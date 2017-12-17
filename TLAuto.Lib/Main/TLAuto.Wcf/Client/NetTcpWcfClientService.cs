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
    public class NetTcpWcfClientService<TProxy> : WcfClientServiceBase<TProxy>
    {
        public NetTcpWcfClientService(string serviceAddress, TimeSpan sendTimeout)
            : base(serviceAddress, sendTimeout) { }

        public NetTcpWcfClientService(string serviceAddress) :
            this(serviceAddress, TimeSpan.FromMinutes(1)) { }

        protected override Binding GetServiceEndpointBindingType()
        {
            return new NetTcpBinding(SecurityMode.None) {MaxReceivedMessageSize = 2147483647};
        }
    }
}