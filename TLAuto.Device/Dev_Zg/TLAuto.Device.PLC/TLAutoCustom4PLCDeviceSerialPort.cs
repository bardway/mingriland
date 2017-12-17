// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Device.PLC.Command.PLC;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC
{
    public class TLAutoCustom4PLCDeviceSerialPort : TLAutoDeviceSerialPortBase, ITLAutoPLCDeviceSerialPort
    {
        private readonly string _logModuleName;
        private readonly int _timeOutMs;

        public TLAutoCustom4PLCDeviceSerialPort(TLAutoDeviceSerialPort tlAutoDeviceSerialPort, string logModuleName = null, int timeOutMs = 5000)
            : base(tlAutoDeviceSerialPort)
        {
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
        }

        public async Task<bool> ChangeDeviceNumber(int deviceNumber)
        {
            var sendCommand = new ChangeDeviceNumberCustom4Command(deviceNumber);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null, _logModuleName, _timeOutMs);
            await tlAutoDeviceSerialPortAsync.InvokeAsync();
            return true;
        }

        public Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatus(int deviceNumber, int itemCount)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatusWithAutoNotification
        (
            int deviceNumber,
            int itemCount,
            int[] querySwitchNumbers,
            int queryTime)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus)
        {
            var sendCommand = new ControlRelayStatusCustom4Command(deviceNumber, relayNumber, relayStatus, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null);
            await tlAutoDeviceSerialPortAsync.InvokeAsync();
            return true;
        }

        public async Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount)
        {
            var sendCommand = new QueryCustom4RelayStatusCommand(deviceNumber, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<IEnumerable<RelayItem>>(TLAutoDeviceSerialPort, sendCommand, null);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public Task<bool> RaiseDigitalSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus swtichStatus)
        {
            throw new NotImplementedException();
        }
    }
}