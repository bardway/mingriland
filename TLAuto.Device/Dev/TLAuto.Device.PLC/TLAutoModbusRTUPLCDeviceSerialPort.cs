// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC
{
    public class TLAutoModbusRTUPLCDeviceSerialPort : TLAutoDeviceSerialPortBase, ITLAutoPLCDeviceSerialPort
    {
        private readonly string _logModuleName;
        private readonly int _timeOutMs;

        public TLAutoModbusRTUPLCDeviceSerialPort(TLAutoDeviceSerialPort tlAutoDeviceSerialPort, string logModuleName = null, int timeOutMs = 5000) :
            base(tlAutoDeviceSerialPort)
        {
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
        }

        public Task<bool> ChangeDeviceNumber(int deviceNumber)
        {
            throw new NotImplementedException();
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

        public Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RaiseDigitalSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus swtichStatus)
        {
            throw new NotImplementedException();
        }
    }
}