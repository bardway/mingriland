// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Device.Projector.Command.MS524;
#endregion

namespace TLAuto.Device.Projector
{
    internal class TLAutoMS524ProjectorDeviceSerialPort : TLAutoDeviceSerialPortBase, ITLAutoProjectorDeviceSerialPort
    {
        private readonly string _logModuleName;
        private readonly int _timeOutMs;

        public TLAutoMS524ProjectorDeviceSerialPort(TLAutoDeviceSerialPort tlAutoDeviceSerialPort, string logModuleName = null, int timeOutMs = 5000) :
            base(tlAutoDeviceSerialPort)
        {
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
        }

        public async Task<bool> PowerOn()
        {
            var sendCommand = new PowerOnCommand();
            var tlSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null, _logModuleName, _timeOutMs);
            return await tlSerialPortAsync.InvokeAsync();
        }

        public async Task<bool> PowerOff()
        {
            var sendCommand = new PowerOffCommand();
            var tlSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null, _logModuleName, _timeOutMs);
            return await tlSerialPortAsync.InvokeAsync();
        }
    }
}