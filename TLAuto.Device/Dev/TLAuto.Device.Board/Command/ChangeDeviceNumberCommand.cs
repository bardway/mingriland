/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using TLAuto.Base.Extensions;

namespace TLAuto.Device.PLC.Command
{
    public class ChangeDeviceNumberCommand : BoardDeviceCommand<bool>
    {
        public ChangeDeviceNumberCommand(int deviceNumber)
            : base(deviceNumber, 0)
        {

        }

        protected override bool GetResult(byte[] bytes)
        {
            return true;
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if (bytes[1] == 221 && bytes[2].ToInt32() == DeviceNumber)
            {
                return true;
            }
            return false;
        }

        protected override int ResultByteLength => 3;

        /// <summary>
        /// 0代表新设备号
        /// </summary>
        protected override string SendCommand => "AA BB FF {0} 5A";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;
    }
}
