// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Text;
#endregion

namespace TLAuto.Device.IoT.ServiceData
{
    [Serializable]
    public class IoTControlServiceData
    {
        public string SignName { set; get; }

        public int DeviceNumber { set; get; }

        public string Key { set; get; }

        public string Value { set; get; }

        public bool HasResult { set; get; }

        public int ResultLength { set; get; }

        public byte[] GetSendBytes()
        {
            return Encoding.Default.GetBytes(Key + "-" + Value + "|");
        }
    }
}