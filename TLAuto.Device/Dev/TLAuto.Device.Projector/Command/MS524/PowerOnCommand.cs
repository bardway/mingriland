// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Projector.Command.MS524
{
    public class PowerOnCommand : ProjectorDeviceCommand<bool>
    {
        protected override string SendCommand => "0D 2A 70 6F 77 3D 6F 6E 23 0D";

        protected override string HeaderCommand => "3E 2A 70 6F 77 3D 6F 6E 23 0D 0D 0A 2A";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;

        //成功：50 4F 57 3D 4F 4E 23 0D 0A
        //失败：42 6C 6F 63 6B 20 69 74 65 6D 23 0D 0A 00
        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            var startIndex = HeaderCommand.Split(' ').Length;
            var endCount = bytes.Length - startIndex;
            switch (endCount)
            {
                case 9:
                    return true;
                case 14:
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
                case 9:
                    if ((bytes[bytes.Length - 1].ToHex() == "0A") &&
                        (bytes[bytes.Length - 2].ToHex() == "0D") &&
                        (bytes[bytes.Length - 3].ToHex() == "23") &&
                        (bytes[bytes.Length - 4].ToHex() == "4E") &&
                        (bytes[bytes.Length - 5].ToHex() == "4F") &&
                        (bytes[bytes.Length - 6].ToHex() == "3D") &&
                        (bytes[bytes.Length - 7].ToHex() == "57") &&
                        (bytes[bytes.Length - 8].ToHex() == "4F") &&
                        (bytes[bytes.Length - 9].ToHex() == "50"))
                    {
                        return true;
                    }
                    return false;
                case 14:
                    if ((bytes[bytes.Length - 1].ToHex() == "00") &&
                        (bytes[bytes.Length - 2].ToHex() == "0A") &&
                        (bytes[bytes.Length - 3].ToHex() == "0D") &&
                        (bytes[bytes.Length - 4].ToHex() == "23") &&
                        (bytes[bytes.Length - 5].ToHex() == "6D") &&
                        (bytes[bytes.Length - 6].ToHex() == "65") &&
                        (bytes[bytes.Length - 7].ToHex() == "74") &&
                        (bytes[bytes.Length - 8].ToHex() == "69") &&
                        (bytes[bytes.Length - 9].ToHex() == "20") &&
                        (bytes[bytes.Length - 10].ToHex() == "6B") &&
                        (bytes[bytes.Length - 11].ToHex() == "63") &&
                        (bytes[bytes.Length - 12].ToHex() == "6F") &&
                        (bytes[bytes.Length - 13].ToHex() == "6C") &&
                        (bytes[bytes.Length - 14].ToHex() == "42"))
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}