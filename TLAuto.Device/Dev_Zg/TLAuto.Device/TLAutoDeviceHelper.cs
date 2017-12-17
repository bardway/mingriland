// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device
{
    public static class TLAutoDeviceHelper
    {
        public static byte[] HexStringToByteArray(string text)
        {
            text = text.Replace(" ", "");
            var buffer = new byte[text.Length / 2];
            for (var i = 0; i < text.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(text.Substring(i, 2), 16);
            }
            return buffer;
        }
    }
}