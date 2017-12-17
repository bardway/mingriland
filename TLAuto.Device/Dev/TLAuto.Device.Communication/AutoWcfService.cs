/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using TLAuto.Device.Communication.Events;

namespace TLAuto.Device.Communication
{
    public abstract class AutoWcfService : IAutoWcfService
    {
        private ServiceHost _host;

        public async Task<bool> StartWcfService(string serviceAddress, Type typeclass, IEnumerable<Type> typeInterfaces)
        {
            ServiceAddress = serviceAddress;
            var isOpen = false;
            if (_host == null)
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _host = new ServiceHost(typeclass);
                        _host.Opened += (s, e) =>
                        {
                            isOpen = true;
                            tokenSource.Cancel();
                        };
                        _host.Faulted += Host_Faulted;
                        _host.Closed += Host_Closed;
                        var serviceEndpointBindingType = GetServiceEndpointBindingType();
                        foreach (var typeInterface in typeInterfaces)
                        {
                            _host.AddServiceEndpoint(typeInterface, serviceEndpointBindingType, serviceAddress);
                        }
                        AddServiceEndpointBehaviors(_host);
                        if (_host.Description.Behaviors.Find<ServiceThrottlingBehavior>() == null)
                        {
                            var behavior = new ServiceThrottlingBehavior
                            {
                                MaxConcurrentCalls = 100,
                                MaxConcurrentSessions = 100,
                                MaxConcurrentInstances = 100
                            };
                            _host.Description.Behaviors.Add(behavior);
                        }
                        var debug = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
                        debug.IncludeExceptionDetailInFaults = true;
                        _host.Open();
                        token.WaitHandle.WaitOne();
                    }
                    catch (Exception ex)
                    {
                        OnError(new AutoWcfServiceErrorMessageEventArgs(ex.Message, ex));
                        tokenSource.Cancel();
                        StopWcfService();
                    }
                }, token);
            }
            return isOpen;
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            OnError(new AutoWcfServiceErrorMessageEventArgs($"ServerHost Faulted：{e}", new FaultException(ServiceAddress)));
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            OnInfo(new AutoWcfServiceMessageEventArgs($"ServerHost Closed：{e}"));
        }

        public void StopWcfService()
        {
            if (_host != null)
            {
                try
                {
                    if (_host.State != CommunicationState.Faulted)
                    {
                        _host.Close();
                        ((IDisposable)_host).Dispose();
                    }
                }
                finally
                {
                    _host = null;
                }
            }
        }

        protected abstract Binding GetServiceEndpointBindingType();

        protected virtual void AddServiceEndpointBehaviors(ServiceHost serviceHost)
        {
        }

        #region Events

        public event EventHandler<AutoWcfServiceMessageEventArgs> Info;

        protected virtual void OnInfo(AutoWcfServiceMessageEventArgs e)
        {
            Info?.Invoke(this, e);
        }

        public event EventHandler<AutoWcfServiceErrorMessageEventArgs> Error;

        protected virtual void OnError(AutoWcfServiceErrorMessageEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        #endregion

        protected string ServiceAddress { private set; get; }
    }
}