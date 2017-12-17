// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.DMX.ServiceData
{
    [Serializable]
    public class DMXControlServiceData
    {
        public int ChannelNum { set; get; }

        public int ChannelValue { set; get; }
    }
}