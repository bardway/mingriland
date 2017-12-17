/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System.Collections.Generic;
using System.Linq;
using TLAuto.Base.Extensions;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC.Command.Switch.SwitchA
{
    public class AutoUploadSwitchStatusCommand : BoardDeviceCommand<SwitchItem>
    {
        private readonly int _switchNumber;

        public AutoUploadSwitchStatusCommand(int deviceNumber, int itemCount, int switchNumber) :
            base(deviceNumber, itemCount)
        {
            _switchNumber = switchNumber;
        }

        internal byte[] GetRaiseSwitchData(SwitchStatus switchStatus)
        {
            var list = new List<byte>();
            list.AddRange("CC".HexStrToBytes(" ").ToList());
            list.AddRange("EE".HexStrToBytes(" ").ToList());
            list.Add(DeviceNumber.ToByte());
            list.Add(_switchNumber.ToByte());
            list.Add(switchStatus.ToInt32().ToByte());
            list.AddRange("3A".HexStrToBytes(" ").ToList());
            return list.ToArray();
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if (bytes[1] == 238 && bytes[2].ToInt32() == DeviceNumber && bytes[3].ToInt32() == _switchNumber && bytes[5] == 58)
            {
                return true;
            }
            return false;
        }

        protected override SwitchItem GetResult(byte[] bytes)
        {
            return new SwitchItem(bytes[3].ToInt32(), bytes[4].ToInt32() == 1 ? SwitchStatus.NC : SwitchStatus.NO);
        }

        protected override int ResultByteLength => 6;

        public override bool HasReceivedData => true;

        public override bool HasSendData => false;
    }
}
