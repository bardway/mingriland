// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Base.Extensions;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.Command.PLC
{
    public class AutoUploadSwitchStatusExCommand : PLCDeviceCommand<SwitchItem>
    {
        public AutoUploadSwitchStatusExCommand(int deviceNumber, int itemCount)
            : base(deviceNumber, itemCount) { }

        protected override int ResultByteLength => 6;

        public override bool HasReceivedData => true;

        public override bool HasSendData => false;

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if ((bytes[1] == 238) && (bytes[2].ToInt32() == DeviceNumber) && (bytes[5] == 58))
            {
                return true;
            }
            return false;
        }

        protected override SwitchItem GetResult(byte[] bytes)
        {
            return new SwitchItem(bytes[3].ToInt32(), bytes[4].ToInt32() == 1 ? SwitchStatus.NC : SwitchStatus.NO);
        }
    }
}