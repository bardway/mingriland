// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endregion

namespace TLAuto.Wcf.Server
{
    public class NetTcpWcfServerService : WcfServerServiceBase
    {
        private readonly bool _portSharingEnabled;
        private readonly TimeSpan _receiveTimeout;

        public NetTcpWcfServerService(TimeSpan receiveTimeout, bool portSharingEnabled = false)
        {
            _receiveTimeout = receiveTimeout;
            _portSharingEnabled = portSharingEnabled;
        }

        public NetTcpWcfServerService(bool portSharingEnabled = false) :
            this(TimeSpan.FromMinutes(10), portSharingEnabled) { }

        protected override Binding GetServiceEndpointBindingType()
        {
            return new NetTcpBinding(SecurityMode.None) {PortSharingEnabled = _portSharingEnabled, ReceiveTimeout = _receiveTimeout};
        }
    }
}