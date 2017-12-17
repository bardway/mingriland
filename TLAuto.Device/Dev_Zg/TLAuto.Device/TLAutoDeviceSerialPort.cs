// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Device.Events;
#endregion

namespace TLAuto.Device
{
    public sealed class TLAutoDeviceSerialPort : IDisposable
    {
        private readonly ConcurrentQueue<byte[]> _sendQueue = new ConcurrentQueue<byte[]>();
        private readonly SerialPort _serialPort = new SerialPort();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public TLAutoDeviceSerialPort(int sendDataTimeSpan = 20)
        {
            SendDataTimeSpan = sendDataTimeSpan;
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;
            _serialPort.DataReceived += SerialPort_DataReceived;
            Task.Factory.StartNew(() => ReadSendQueue(_tokenSource.Token), _tokenSource.Token);
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Eof)
            {
                return;
            }
            var readLength = _serialPort.BytesToRead;
            if (readLength != 0)
            {
                var buffer = new byte[readLength];
                _serialPort.Read(buffer, 0, readLength);
                OnDataReceived(new TLAutoDeviceDataReceivedEventArgs(buffer));
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new Exception(e.EventType.ToString());
        }

        private void ReadSendQueue(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                if (!_sendQueue.IsEmpty)
                {
                    byte[] data;
                    if (_sendQueue.TryDequeue(out data))
                    {
                        _serialPort.Write(data, 0, data.Length);
                    }
                }
                Thread.Sleep(SendDataTimeSpan);
            }
        }

        public void Open(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.Parity = parity;
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = stopBits;
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void Send(byte[] data)
        {
            _sendQueue.Enqueue(data);
        }

        /// <summary>
        /// 模拟数据接收
        /// </summary>
        /// <param name="data"></param>
        public void RaiseDataReceived(byte[] data)
        {
            OnDataReceived(new TLAutoDeviceDataReceivedEventArgs(data));
        }

        #region Events
        public event EventHandler<TLAutoDeviceDataReceivedEventArgs> DataReceived;

        private void OnDataReceived(TLAutoDeviceDataReceivedEventArgs e)
        {
            var handler = DataReceived;
            handler?.Invoke(this, e);
        }
        #endregion

        #region Properties
        private volatile int _sendDataTimeSpan;

        public int SendDataTimeSpan { set => _sendDataTimeSpan = value; get => _sendDataTimeSpan; }
        #endregion
    }
}