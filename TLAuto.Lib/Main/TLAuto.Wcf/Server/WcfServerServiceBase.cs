// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Wcf.Server
{
    public abstract class WcfServerServiceBase : IWcfServerService
    {
        private ServiceHost _host;

        protected string ServiceAddress { private set; get; }

        public async Task<bool> StartWcfService(string serviceAddress, Type typeClass, params Type[] typeInterfaces)
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
                                                    _host = new ServiceHost(typeClass);
                                                    _host.Opened += (s, e) =>
                                                                    {
                                                                        isOpen = true;
                                                                        tokenSource.Cancel();
                                                                        OnOpened();
                                                                    };
                                                    _host.Faulted += Host_Faulted;
                                                    _host.Closed += Host_Closed;
                                                    var serviceEndpointBindingType = GetServiceEndpointBindingType();
                                                    foreach (var typeInterface in typeInterfaces)
                                                    {
                                                        _host.AddServiceEndpoint(typeInterface, serviceEndpointBindingType, serviceAddress);
                                                    }
                                                    AddServiceBehaviors(_host);
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
                                                    AddServiceEndpointBehaviors(_host);
                                                    _host.Open();
                                                    token.WaitHandle.WaitOne();
                                                }
                                                catch (Exception ex)
                                                {
                                                    OnError(new WcfServerServiceErrorMessageEventArgs(ex.Message, ex));
                                                    tokenSource.Cancel();
                                                    StopWcfService();
                                                }
                                            },
                                            token);
            }
            return isOpen;
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

        private void Host_Faulted(object sender, EventArgs e)
        {
            OnError(new WcfServerServiceErrorMessageEventArgs($"ServerHost Faulted：{e}", new FaultException(ServiceAddress)));
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            OnClosed(new WcfServerServiceMessageEventArgs($"ServerHost Closed：{e}"));
        }

        protected abstract Binding GetServiceEndpointBindingType();

        protected virtual void AddServiceBehaviors(ServiceHost serviceHost) { }

        protected virtual void AddServiceEndpointBehaviors(ServiceHost serviceHost) { }

        #region Events
        public event EventHandler<WcfServerServiceMessageEventArgs> Closed;

        protected virtual void OnClosed(WcfServerServiceMessageEventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        public event EventHandler<WcfServerServiceErrorMessageEventArgs> Error;

        protected virtual void OnError(WcfServerServiceErrorMessageEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler Opened;

        protected virtual void OnOpened()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}