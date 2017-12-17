// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.PLC.Command.PLC
{
    public class ChangeDeviceNumberCustom4Command : PLCDeviceCommand<bool>
    {
        public ChangeDeviceNumberCustom4Command(int deviceNumber)
            : base(deviceNumber, 0) { }

        protected override int ResultByteLength => 2;

        /// <summary>
        /// 0代表新设备号
        /// </summary>
        protected override string SendCommand => "AA 5A 00 Fb {0} FF";

        public override bool HasSendData => true;

        public override bool HasReceivedData => false;

        protected override bool HasSendCRC => false;

        protected override bool GetResult(byte[] bytes)
        {
            return true;
        }

        protected override bool CheckReveicedDataHeader(byte[] bytes, out int headerLength)
        {
            headerLength = 1;
            return bytes[0] == 65;
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if (bytes[1] == 65)
            {
                return true;
            }
            return false;
        }
    }
}