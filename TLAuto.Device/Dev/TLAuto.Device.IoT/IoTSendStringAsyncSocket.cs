// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Text;
using System.Threading;

using TLAuto.Base.Async;
using TLAuto.Base.Network;
using TLAuto.Base.Network.Events;
using TLAuto.Device.IoT.ServiceData;
using TLAuto.Log;
#endregion

namespace TLAuto.Device.IoT
{
    public class IoTSendStringAsyncSocket : AsyncTaskBase<string>
    {
        private readonly object _lockObj = new object();
        private readonly LogWraper _log = new LogWraper("IoTTestConnectStatusAsyncSocket");
        private readonly IoTControlServiceData _serviceData;
        private INetworkBaseWrapper _networkWrapper;
        private string _receiveString = string.Empty;

        public IoTSendStringAsyncSocket(INetworkBaseWrapper networkWrapper, IoTControlServiceData serviceData, CancellationToken? cancelToken, int timeOutMs = 4000)
            : base(cancelToken, timeOutMs)
        {
            _serviceData = serviceData;
            _networkWrapper = networkWrapper;
            _networkWrapper.DataReceived += NetworkWrapper_DataReceived;
        }

        private void NetworkWrapper_DataReceived(object sender, NetworkBaseDataReceivedEventArgs e)
        {
            lock (_lockObj)
            {
                var result = Encoding.Default.GetString(e.Data);
                _receiveString += result;
                if (_receiveString.Length == _serviceData.ResultLength)
                {
                    PostResult(_receiveString);
                }
            }
        }

        protected override void Invoke()
        {
            _networkWrapper.Send(_serviceData.GetSendBytes());
            if (!_serviceData.HasResult)
            {
                PostResult("true");
            }
        }

        protected override void Exception(Exception ex)
        {
            _log.Error(ex.Message, ex);
        }

        protected override void Dispose(bool disposing)
        {
            lock (_lockObj)
            {
                _networkWrapper.DataReceived -= NetworkWrapper_DataReceived;
                _networkWrapper = null;
            }
        }
    }
}