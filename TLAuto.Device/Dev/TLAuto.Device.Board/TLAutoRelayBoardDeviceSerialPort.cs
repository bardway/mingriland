/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLAuto.Device.PLC.Command;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;
using TLAuto.Device.PLC.Command.Relay.RelayA;

namespace TLAuto.Device.PLC
{
    public class TLAutoRelayBoardDeviceSerialPort : TLAutoDeviceSerialPort, ITLAutoRelayBoardDeviceSerialPort
    {
        public async Task<bool> ChangeDeviceNumber(int deviceNumber)
        {
            var sendCommand = new ChangeDeviceNumberCommand(deviceNumber);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(this, sendCommand, null, 5000);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }

        public async Task<bool> ControlRelay(int deviceNumber, int itemCount, int relayNumber, RelayStatus relayStatus)
        {
            var sendCommand = new ControlRelayStatusCommand(deviceNumber, relayNumber, relayStatus, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<bool>(this, sendCommand, null);
            await tlAutoDeviceSerialPortAsync.InvokeAsync();
            return true;
        }

        public async Task<IEnumerable<RelayItem>> QueryRelayStatus(int deviceNumber, int itemCount)
        {
            var sendCommand = new QueryRelayStatusCommand(deviceNumber, itemCount);
            var tlAutoDeviceSerialPortAsync = new TLAutoDeviceSerialPortAsync<IEnumerable<RelayItem>>(this, sendCommand, null);
            return await tlAutoDeviceSerialPortAsync.InvokeAsync();
        }
    }
}
