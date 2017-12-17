// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Device.PLC.Command.PLC;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC
{
    public class TLAutoCustom1PLCDeviceSerialPort : TLAutoCustomBasePLCDeviceSerialPort
    {
        private readonly string _logModuleName;
        private readonly int _timeOutMs;

        public TLAutoCustom1PLCDeviceSerialPort(TLAutoDeviceSerialPort tlAutoDeviceSerialPort, bool hasControlRelayResultStatus, string logModuleName = null, int timeOutMs = 5000) :
            base(tlAutoDeviceSerialPort, hasControlRelayResultStatus)
        {
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
        }

        public override async Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatus(int deviceNumber, int itemCount)
        {
            var sendCommand = new QueryCustom1SwitchStatusCommand(deviceNumber, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<IEnumerable<SwitchItem>>(TLAutoDeviceSerialPort, sendCommand, null, _logModuleName, _timeOutMs);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public override async Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount)
        {
            var sendCommand = new QueryCustom1RelayStatusCommand(deviceNumber, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<IEnumerable<RelayItem>>(TLAutoDeviceSerialPort, sendCommand, null);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }
    }
}