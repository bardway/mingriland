// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device
{
    public class TLAutoDeviceSerialPortBase
    {
        public TLAutoDeviceSerialPortBase(TLAutoDeviceSerialPort tlAutoDeviceSerialPort)
        {
            TLAutoDeviceSerialPort = tlAutoDeviceSerialPort;
        }

        public TLAutoDeviceSerialPort TLAutoDeviceSerialPort { get; }
    }
}