// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Wcf.Client
{
    public abstract class WcfClientServiceBase<TProxy> : IWcfClientService<TProxy>
    {
        private readonly TimeSpan _sendTimeout;
        private TProxy _proxy;

        protected WcfClientServiceBase(string serviceAddress, TimeSpan sendTimeout)
        {
            ServiceAddress = serviceAddress;
            _sendTimeout = sendTimeout;
        }

        protected string ServiceAddress { get; }

        public virtual bool Send(Action<TProxy> action)
        {
            bool result;
            var serviceBinding = GetInitServiceEndpointBindingType();
            using (var channelFactory = GetChannelFactory(serviceBinding))
            {
                AddServiceEndpointBehaviors(channelFactory);
                _proxy = channelFactory.CreateChannel();
                result = Invoke(_proxy, action);
                Close();
            }
            return result;
        }

        public virtual TReturn Send<TReturn>(Func<TProxy, TReturn> func)
        {
            TReturn t;
            var serviceBinding = GetInitServiceEndpointBindingType();
            using (var channelFactory = GetChannelFactory(serviceBinding))
            {
                AddServiceEndpointBehaviors(channelFactory);
                _proxy = channelFactory.CreateChannel();
                t = Invoke(_proxy, func);
                Close();
            }
            return t;
        }

        public virtual async Task<TReturn> SendAsync<TReturn>(Func<TProxy, Task<TReturn>> funcAsync)
        {
            TReturn t;
            var serviceBinding = GetInitServiceEndpointBindingType();
            using (var channelFactory = GetChannelFactory(serviceBinding))
            {
                AddServiceEndpointBehaviors(channelFactory);
                _proxy = channelFactory.CreateChannel();
                t = await InvokeAsync(_proxy, funcAsync);
                Close();
            }
            return t;
        }

        protected abstract Binding GetServiceEndpointBindingType();

        protected Binding GetInitServiceEndpointBindingType()
        {
            var serviceBinding = GetServiceEndpointBindingType();
            serviceBinding.SendTimeout = _sendTimeout;
            return serviceBinding;
        }

        protected virtual void AddServiceEndpointBehaviors(ChannelFactory<TProxy> channelFactory) { }

        protected virtual ChannelFactory<TProxy> GetChannelFactory(Binding serviceBinding)
        {
            return new ChannelFactory<TProxy>(serviceBinding, ServiceAddress);
        }

        protected bool Invoke(TProxy proxy, Action<TProxy> action)
        {
            try
            {
                action(proxy);
                return true;
            }
            catch (CommunicationException ce)
            {
                ((ICommunicationObject)proxy).Abort();
                OnCommunicationError(new WcfClientServiceErrorEventArgs("通信异常", ce));
            }
            catch (TimeoutException te)
            {
                ((ICommunicationObject)proxy).Abort();
                OnTimeoutError(new WcfClientServiceErrorEventArgs("通信超时", te));
            }
            catch (Exception ex)
            {
                OnError(new WcfClientServiceErrorEventArgs("通信错误", ex));
            }
            return false;
        }

        protected TReturn Invoke<TReturn>(TProxy proxy, Func<TProxy, TReturn> func)
        {
            var returnValue = default(TReturn);
            try
            {
                returnValue = func(proxy);
            }
            catch (CommunicationException ce)
            {
                ((ICommunicationObject)proxy).Abort();
                OnCommunicationError(new WcfClientServiceErrorEventArgs("通信异常", ce));
            }
            catch (TimeoutException te)
            {
                ((ICommunicationObject)proxy).Abort();
                OnTimeoutError(new WcfClientServiceErrorEventArgs("通信超时", te));
            }
            catch (Exception ex)
            {
                OnError(new WcfClientServiceErrorEventArgs("通信错误", ex));
            }
            return returnValue;
        }

        protected async Task<TReturn> InvokeAsync<TReturn>(TProxy proxy, Func<TProxy, Task<TReturn>> funcAsync)
        {
            var returnValue = default(TReturn);
            try
            {
                returnValue = await funcAsync(proxy);
            }
            catch (CommunicationException ce)
            {
                ((ICommunicationObject)proxy).Abort();
                OnCommunicationError(new WcfClientServiceErrorEventArgs("通信异常", ce));
            }
            catch (TimeoutException te)
            {
                ((ICommunicationObject)proxy).Abort();
                OnTimeoutError(new WcfClientServiceErrorEventArgs("通信超时", te));
            }
            catch (Exception ex)
            {
                OnError(new WcfClientServiceErrorEventArgs("通信错误", ex));
            }
            return returnValue;
        }

        protected void Close()
        {
            if (_proxy != null)
            {
                ((ICommunicationObject)_proxy).Abort();
                ((ICommunicationObject)_proxy).Close();
            }
        }

        #region Events
        public event EventHandler<WcfClientServiceErrorEventArgs> CommunicationError;

        public event EventHandler<WcfClientServiceErrorEventArgs> TimeoutError;

        public event EventHandler<WcfClientServiceErrorEventArgs> Error;

        protected virtual void OnCommunicationError(WcfClientServiceErrorEventArgs e)
        {
            CommunicationError?.Invoke(this, e);
        }

        protected virtual void OnTimeoutError(WcfClientServiceErrorEventArgs e)
        {
            TimeoutError?.Invoke(this, e);
        }

        protected virtual void OnError(WcfClientServiceErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }
        #endregion
    }
}