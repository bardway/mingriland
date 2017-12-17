/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.IO.Ports;
using TLAuto.Device.Ports.Events;

namespace TLAuto.Device.Ports
{
    public class TLSerialPort : ITLSerialPort
    {
        private readonly SerialPort _serialPort = new SerialPort();

        public TLSerialPort()
        {
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;
            _serialPort.Disposed += SerialPort_Disposed;
        }

        ~TLSerialPort()
        {
            _serialPort.Dispose();
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

        public void Dispose()
        {
            _serialPort.Dispose();
        }

        public void Send(byte[] data)
        {
            _serialPort.Write(data, 0, data.Length);
        }

        private void SerialPort_Disposed(object sender, EventArgs e)
        {
            OnDisposed();
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnErrorReceived(new TLSerialErrorReceivedEventArgs(e.EventType.ToString()));
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Eof)
                return;
            var readLength = _serialPort.BytesToRead;
            if (readLength != 0)
            {
                var buffer = new byte[readLength];
                _serialPort.Read(buffer, 0, readLength);
                OnDataReceived(new TLDataReceivedEventArgs(buffer));
            }
        }

        #region Events

        public event EventHandler<TLDataReceivedEventArgs> DataReceived;
        private void OnDataReceived(TLDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public event EventHandler<TLSerialErrorReceivedEventArgs> ErrorReceived;
        private void OnErrorReceived(TLSerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(this, e);
        }

        public event EventHandler Disposed;
        private void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public bool IsOpen => _serialPort.IsOpen;
    }
}