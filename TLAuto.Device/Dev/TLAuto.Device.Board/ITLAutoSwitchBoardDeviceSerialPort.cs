/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC
{
    public interface ITLAutoSwitchBoardDeviceSerialPort
    {
        Task<bool> ChangeDeviceNumber(int deviceNumber);

        Task<IEnumerable<SwitchItem>> QuerySwitchStatus(int deviceNumber, int itemCount);

        Task<IEnumerable<SwitchItem>> QuerySwitchStatusWithAutoNotification(int deviceNumber, int itemCount, int[] querySwitchNumbers, int queryTime);

        Task<bool> RaiseSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus switchStatus);
    }
}
