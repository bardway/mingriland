/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Linq;
using TLAuto.Base.Extensions;

namespace TLAuto.Device.PLC
{
    public static class TLAutoBoardDeviceHelper
    {
        public static string GetCRCFromHexs(string hex)
        {
            var numbers = hex.Split(' ');
            var result = numbers.Sum(t => Convert.ToInt32(t, 16));
            return Convert.ToString(result % 256, 16).PadLeft(2, '0');
        }

        public static bool IsRightCRC(byte[] buffer, int endCRCLength)
        {
            var result = 0;
            for (var i = 0; i < endCRCLength - 1; i++)
            {
                result += Convert.ToInt32(buffer[i].ToHex(), 16);
            }
            var hex = Convert.ToString(result % 256, 16).PadLeft(2, '0');
            var crc = Convert.ToByte(hex, 16);
            return buffer[endCRCLength - 1] == crc;
        }
    }
}
