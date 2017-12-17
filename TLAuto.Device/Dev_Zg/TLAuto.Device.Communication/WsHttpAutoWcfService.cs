/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace TLAuto.Device.Communication
{
    public sealed class WsHttpAutoWcfService : AutoWcfService
    {
        protected override Binding GetServiceEndpointBindingType()
        {
            return new WSHttpBinding(SecurityMode.None);
        }

        protected override void AddServiceEndpointBehaviors(ServiceHost serviceHost)
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
        }
    }
}