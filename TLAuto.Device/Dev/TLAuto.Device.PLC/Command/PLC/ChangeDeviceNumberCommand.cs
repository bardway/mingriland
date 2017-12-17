// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.PLC.Command.PLC
{
    public class ChangeDeviceNumberCommand : PLCDeviceCommand<bool>
    {
        public ChangeDeviceNumberCommand(int deviceNumber)
            : base(deviceNumber, 0) { }

        protected override int ResultByteLength => 3;

        /// <summary>
        /// 0代表新设备号
        /// </summary>
        protected override string SendCommand => "AA BB FF {0} 5A";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;

        protected override bool GetResult(byte[] bytes)
        {
            return true;
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if ((bytes[1] == 221) && (bytes[2].ToInt32() == DeviceNumber))
            {
                return true;
            }
            return false;
        }
    }
}