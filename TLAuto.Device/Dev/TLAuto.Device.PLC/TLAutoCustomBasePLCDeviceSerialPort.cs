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
    public abstract class TLAutoCustomBasePLCDeviceSerialPort : TLAutoDeviceSerialPortBase, ITLAutoPLCDeviceSerialPort
    {
        private readonly bool _hasControlRelayResultStatus;
        private readonly string _logModuleName;
        private readonly int _timeOutMs;

        protected TLAutoCustomBasePLCDeviceSerialPort(TLAutoDeviceSerialPort tlAutoDeviceSerialPort, bool hasControlRelayResultStatus, string logModuleName = null, int timeOutMs = 5000) :
            base(tlAutoDeviceSerialPort)
        {
            _hasControlRelayResultStatus = hasControlRelayResultStatus;
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
        }

        public virtual async Task<bool> ChangeDeviceNumber(int deviceNumber)
        {
            var sendCommand = new ChangeDeviceNumberCommand(deviceNumber);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null, _logModuleName, _timeOutMs);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public abstract Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatus(int deviceNumber, int itemCount);

        public virtual async Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatusWithAutoNotification
        (
            int deviceNumber,
            int itemCount,
            int[] querySwitchNumbers,
            int queryTime)
        {
            var tasks = new List<Task<SwitchItem>>();
            Parallel.ForEach(querySwitchNumbers,
                             switchNumber =>
                             {
                                 var sendCommand = new AutoUploadSwitchStatusCommand(deviceNumber, itemCount, switchNumber);
                                 var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<SwitchItem>(TLAutoDeviceSerialPort, sendCommand, null, null, queryTime);
                                 var task = tlAutoDeviceSerialPortAsync.InvokeAsync();
                                 tasks.Add(task);
                             });
            var switchItems = await Task.WhenAll(tasks.ToArray());
            return switchItems;
        }

        public virtual async Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus)
        {
            var sendCommand = new ControlRelayStatusCommand(deviceNumber, relayNumber, relayStatus, itemCount, _hasControlRelayResultStatus);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(TLAutoDeviceSerialPort, sendCommand, null);
            var result = await tlAutoDeviceSerialPortAsync.InvokeAsync();
            if (_hasControlRelayResultStatus)
            {
                return result;
            }
            return true;
        }

        public abstract Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount);

        public virtual async Task<bool> RaiseDigitalSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus swtichStatus)
        {
            var sendCommand = new AutoUploadSwitchStatusCommand(deviceNumber, itemCount, switchNumber);
            var buffer = sendCommand.GetRaiseSwitchData(swtichStatus);
            TLAutoDeviceSerialPort.RaiseDataReceived(buffer);
            return await Task.Factory.StartNew(() => true);
        }
    }
}