// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Projector.ServiceData
{
    [Serializable]
    public class ProjectorControlServiceData
    {
        public string PortSignName { set; get; }

        public int DeviceNumber { set; get; }

        public bool PowerOnOrOff { set; get; }
    }
}