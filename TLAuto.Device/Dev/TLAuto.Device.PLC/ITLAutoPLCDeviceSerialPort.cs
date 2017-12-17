// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC
{
    public interface ITLAutoPLCDeviceSerialPort
    {
        Task<bool> ChangeDeviceNumber(int deviceNumber);

        Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatus(int deviceNumber, int itemCount);

        Task<IEnumerable<SwitchItem>> QueryDigitalSwitchStatusWithAutoNotification(int deviceNumber, int itemCount, int[] querySwitchNumbers, int queryTime);

        Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus);

        Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount);

        Task<bool> RaiseDigitalSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus swtichStatus);
    }
}