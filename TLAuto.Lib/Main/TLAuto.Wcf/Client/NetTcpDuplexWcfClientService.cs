// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Wcf.Client
{
    public class NetTcpDuplexWcfClientService<TProxy> : NetTcpWcfClientService<TProxy>
    {
        private readonly object _implementation;
        private DuplexChannelFactory<TProxy> _channelFactory;
        private TProxy _proxy;

        public NetTcpDuplexWcfClientService(object implementation, string serviceAddress, TimeSpan sendTimeout)
            : base(serviceAddress, sendTimeout)
        {
            _implementation = implementation;
        }

        public NetTcpDuplexWcfClientService(object implementation, string serviceAddress) :
            this(implementation, serviceAddress, TimeSpan.FromMinutes(1)) { }

        protected override ChannelFactory<TProxy> GetChannelFactory(Binding serviceBinding)
        {
            return _channelFactory ?? (_channelFactory = new DuplexChannelFactory<TProxy>(new InstanceContext(_implementation), serviceBinding, ServiceAddress));
        }

        public override bool Send(Action<TProxy> action)
        {
            var serviceBinding = GetInitServiceEndpointBindingType();
            var channelFactory = GetChannelFactory(serviceBinding);
            if (_proxy == null)
            {
                _proxy = channelFactory.CreateChannel();
            }
            return Invoke(_proxy, action);
        }

        public override TReturn Send<TReturn>(Func<TProxy, TReturn> func)
        {
            var serviceBinding = GetInitServiceEndpointBindingType();
            var channelFactory = GetChannelFactory(serviceBinding);
            if (_proxy == null)
            {
                _proxy = channelFactory.CreateChannel();
            }
            var t = Invoke(_proxy, func);
            return t;
        }

        public override async Task<TReturn> SendAsync<TReturn>(Func<TProxy, Task<TReturn>> funcAsync)
        {
            var serviceBinding = GetInitServiceEndpointBindingType();
            var channelFactory = GetChannelFactory(serviceBinding);
            if (_proxy == null)
            {
                _proxy = channelFactory.CreateChannel();
            }
            var t = await InvokeAsync(_proxy, funcAsync);
            return t;
        }

        public new void Close()
        {
            base.Close();
            //_proxy = default(TProxy);
            //(_channelFactory as IDisposable)?.Dispose();
            //_channelFactory = null;
        }
    }
}