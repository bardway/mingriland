/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace TLAuto.Device.Communication
{
    public abstract class SendWcfCommand<TProxy, TReturn>
    {
        private TProxy _proxy;

        protected SendWcfCommand(string serviceAddress)
        {
            ServiceAddress = serviceAddress;
        }

        public async Task<TReturn> Send(Func<TProxy, Task<TReturn>> func)
        {
            TReturn t;
            using (var channelFactory = new ChannelFactory<TProxy>(new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = SendTimeout,
                MaxReceivedMessageSize = 2147483647,
            }, ServiceAddress))
            {
                _proxy = channelFactory.CreateChannel();
                t = await Invoke(_proxy, func);
                Close();
            }
            return t;
        }

        public void Close()
        {
            if (_proxy != null)
            {
                ((ICommunicationObject)_proxy).Abort();
                ((ICommunicationObject)_proxy).Close();
            }
        }

        private async Task<TReturn> Invoke(TProxy proxy, Func<TProxy, Task<TReturn>> func)
        {
            TReturn returnValue = default(TReturn);
            try
            {
                returnValue = await func(proxy);
            }
            catch (CommunicationException ce)
            {
                ((ICommunicationObject)proxy).Abort();
                LogCommunicationException(ce);
                //_writeLogMsgAction("通信异常，错误为：" + ce.Message);
            }
            catch (TimeoutException te)
            {
                ((ICommunicationObject)proxy).Abort();
                LogTimeoutException(te);
                //_writeLogMsgAction("通信超时，错误为：" + te.Message);
            }
            catch (Exception ex)
            {
                LogException(ex);
                //_writeLogMsgAction("通信错误，错误为：" + ex.Message);
            }
            return returnValue;
        }

        protected abstract void LogCommunicationException(CommunicationException ce);

        protected abstract void LogTimeoutException(TimeoutException te);

        protected abstract void LogException(Exception ce);

        public string ServiceAddress { get; }

        public TimeSpan SendTimeout { set; get; } = TimeSpan.FromMinutes(1);
    }
}