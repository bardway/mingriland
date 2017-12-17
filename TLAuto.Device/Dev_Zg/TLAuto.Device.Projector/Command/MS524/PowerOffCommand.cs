// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Projector.Command.MS524
{
    public class PowerOffCommand : ProjectorDeviceCommand<bool>
    {
        protected override string SendCommand => "0D 2A 70 6F 77 3D 6F 66 66 23 0D";

        protected override string HeaderCommand => "3E 2A 70 6F 77 3D 6F 66 66 23 0D 0D 0A";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;

        //成功：2A 50 4F 57 3D 4F 46 46 23 0D 0A
        //失败：无
        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            var startIndex = HeaderCommand.Split(' ').Length;
            var endCount = bytes.Length - startIndex;
            switch (endCount)
            {
                case 11:
                    return true;
                case 0:
                    return true;
                default:
                    return false;
            }
        }

        protected override bool GetResult(byte[] bytes)
        {
            var startIndex = HeaderCommand.Split(' ').Length;
            var endCount = bytes.Length - startIndex;
            switch (endCount)
            {
                case 11:
                    if ((bytes[bytes.Length - 1].ToHex() == "0A") &&
                        (bytes[bytes.Length - 2].ToHex() == "0D") &&
                        (bytes[bytes.Length - 3].ToHex() == "23") &&
                        (bytes[bytes.Length - 4].ToHex() == "46") &&
                        (bytes[bytes.Length - 5].ToHex() == "46") &&
                        (bytes[bytes.Length - 6].ToHex() == "4F") &&
                        (bytes[bytes.Length - 7].ToHex() == "3D") &&
                        (bytes[bytes.Length - 8].ToHex() == "57") &&
                        (bytes[bytes.Length - 9].ToHex() == "4F") &&
                        (bytes[bytes.Length - 10].ToHex() == "50") &&
                        (bytes[bytes.Length - 11].ToHex() == "2A"))
                    {
                        return true;
                    }
                    return false;
                case 0:
                    return true;
                default:
                    return false;
            }
        }
    }
}