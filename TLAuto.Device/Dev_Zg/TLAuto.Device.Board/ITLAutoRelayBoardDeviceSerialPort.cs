/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC
{
    public interface ITLAutoRelayBoardDeviceSerialPort
    {
        Task<bool> ChangeDeviceNumber(int deviceNumber);

        Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus);

        Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount);
    }
}
