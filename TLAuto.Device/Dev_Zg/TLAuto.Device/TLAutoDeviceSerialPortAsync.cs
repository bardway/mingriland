// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading;

using TLAuto.Base.Async;
using TLAuto.Base.Extensions;
using TLAuto.Device.Command;
using TLAuto.Device.Events;
using TLAuto.Log;
#endregion

namespace TLAuto.Device
{
    public class TLAutoDeviceSerialPortAsync<T> : AsyncTaskBase<T>
    {
        private readonly object _lockObj = new object();
        private readonly LogWraper _log;
        private DeviceCommand<T> _deviceCommand;
        private TLAutoDeviceSerialPort _tlAutoDeviceSerialPort;

        public TLAutoDeviceSerialPortAsync
        (
            TLAutoDeviceSerialPort tlAutoDeviceSerialPort,
            DeviceCommand<T> deviceCommand,
            CancellationToken? cancelToken,
            string logModuleName = null,
            int timeOutMs = 4000)
            : base(cancelToken, timeOutMs)
        {
            _tlAutoDeviceSerialPort = tlAutoDeviceSerialPort;
            _tlAutoDeviceSerialPort.DataReceived += TLAutoDeviceSerialPortDataReceived;
            _deviceCommand = deviceCommand;
            _log = new LogWraper(logModuleName.IsNullOrEmpty() ? "TLAutoDeviceSerialPortAsync" : logModuleName);
        }

        private void TLAutoDeviceSerialPortDataReceived(object sender, TLAutoDeviceDataReceivedEventArgs e)
        {
            lock (_lockObj)
            {
                T t;
                if (_deviceCommand != null)
                {
                    if (_deviceCommand.ParseReceivedData(e.Bytes, out t))
                    {
                        PostResult(t);
                    }
                }
            }
        }

        protected override void Invoke()
        {
            if (_deviceCommand.HasSendData)
            {
                var data = _deviceCommand.GetSendData();
                _tlAutoDeviceSerialPort.Send(data);
                if (!_deviceCommand.HasReceivedData)
                {
                    PostResult(default(T));
                }
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
                _tlAutoDeviceSerialPort.DataReceived -= TLAutoDeviceSerialPortDataReceived;
                _tlAutoDeviceSerialPort = null;
                _deviceCommand = null;
            }
        }
    }
}