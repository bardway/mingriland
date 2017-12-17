/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using TLAuto.Device.PLC.Command;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;
using TLAuto.Device.PLC.Command.Switch.SwitchA;

namespace TLAuto.Device.PLC
{
    public class TLAutoSwitchBoardDeviceSerialPort : TLAutoDeviceSerialPort, ITLAutoSwitchBoardDeviceSerialPort
    {
        public async Task<bool> ChangeDeviceNumber(int deviceNumber)
        {
            var sendCommand = new ChangeDeviceNumberCommand(deviceNumber);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(this, sendCommand, null, 5000);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public async Task<IEnumerable<SwitchItem>> QuerySwitchStatus(int deviceNumber, int itemCount)
        {
            var sendCommand = new QuerySwitchStatusCommand(deviceNumber, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<IEnumerable<SwitchItem>>(this, sendCommand, null, 5000);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public async Task<IEnumerable<SwitchItem>> QuerySwitchStatusWithAutoNotification(int deviceNumber, int itemCount, int[] querySwitchNumbers, int queryTime)
        {
            var tasks = new List<Task<SwitchItem>>();
            Parallel.ForEach(querySwitchNumbers, switchNumber =>
            {
                var sendCommand = new AutoUploadSwitchStatusCommand(deviceNumber, itemCount, switchNumber);
                var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<SwitchItem>(this, sendCommand, null, queryTime);
                var task = tlAutoDeviceSerialPortAsync.InvokeAsync();
                tasks.Add(task);
            });
            var switchItems = await Task.WhenAll(tasks.ToArray());
            return switchItems;
        }

        public async Task<bool> RaiseSwitchStatus(int deviceNumber, int itemCount, int switchNumber, SwitchStatus switchStatus)
        {
            var sendCommand = new AutoUploadSwitchStatusCommand(deviceNumber, itemCount, switchNumber);
            var buffer = sendCommand.GetRaiseSwitchData(switchStatus);
            RaiseDataReceived(buffer);
            return await Task.Factory.StartNew(() => true);
        }
    }
}
