/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.IO.Ports;
using TLAuto.Device.Ports.Events;

namespace TLAuto.Device.Ports
{
    public interface ITLSerialPort : IDisposable
    {
        void Open(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits);

        void Close();

        void Send(byte[] data);

        event EventHandler<TLDataReceivedEventArgs> DataReceived;

        event EventHandler<TLSerialErrorReceivedEventArgs> ErrorReceived;

        event EventHandler Disposed;

        bool IsOpen { get; }
    }
}