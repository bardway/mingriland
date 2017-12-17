// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO.Ports;

using TLAuto.Base.Extensions;
using TLAuto.Base.Network.Events;
#endregion

namespace TLAuto.Base.Network.Ports
{
    public class SerialPortWrapper : INetworkBaseWrapper
    {
        private readonly SerialPort _serialPort = new SerialPort();

        public SerialPortWrapper()
        {
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        public SerialPortWrapper(string portName, BaudRate baudRate = BaudRate.BaudRate9600, Parity parity = Parity.None, Databits dataBits = Databits.EightBits, StopBits stopBits = StopBits.One) : this()
        {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
        }

        public string PortName { set => _serialPort.PortName = value; get => _serialPort.PortName; }

        public BaudRate BaudRate { set => _serialPort.BaudRate = value.ToInt32(); get => (BaudRate)_serialPort.BaudRate; }

        public Parity Parity { set => _serialPort.Parity = value; get => _serialPort.Parity; }

        public Databits DataBits { set => _serialPort.DataBits = value.ToInt32(); get => (Databits)_serialPort.DataBits; }

        public StopBits StopBits { set => _serialPort.StopBits = value; get => _serialPort.StopBits; }

        public bool Connect()
        {
            try
            {
                _serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs("Connec failed.", ex));
            }
            return false;
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            Close();
            if (_serialPort.IsOpen)
            {
                _serialPort.Dispose();
            }
        }

        public bool Send(byte[] data)
        {
            if (!_serialPort.IsOpen)
            {
                return false;
            }
            try
            {
                _serialPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs("Send data failed.", ex));
            }
            return false;
        }

        public bool IsConnected => _serialPort.IsOpen;

        ~SerialPortWrapper()
        {
            Dispose();
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnError(new NetworkBaseErrorEventArgs($"Received Error:{e.EventType}", null));
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
                OnDataReceived(new NetworkBaseDataReceivedEventArgs(buffer));
            }
        }

        public bool Send(string data)
        {
            if (!_serialPort.IsOpen)
            {
                return false;
            }
            _serialPort.WriteLine(data);
            return true;
        }

        #region Events
        public event EventHandler Connected;

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<NetworkBaseDataReceivedEventArgs> DataReceived;

        protected virtual void OnDataReceived(NetworkBaseDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public event EventHandler<NetworkBaseErrorEventArgs> Error;

        protected virtual void OnError(NetworkBaseErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler<NetworkDisconnectedEventArgs> Disconnected;

        protected virtual void OnDisconnected(NetworkDisconnectedEventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }
        #endregion
    }
}