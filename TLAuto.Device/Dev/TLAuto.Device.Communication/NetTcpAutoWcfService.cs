/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace TLAuto.Device.Communication
{
    public class NetTcpAutoWcfService : AutoWcfService
    {
        private readonly bool _portSharingEnabled;

        public NetTcpAutoWcfService(bool portSharingEnabled = false)
        {
            _portSharingEnabled = portSharingEnabled;
        }

        protected override Binding GetServiceEndpointBindingType()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = _portSharingEnabled };
        }
    }
}