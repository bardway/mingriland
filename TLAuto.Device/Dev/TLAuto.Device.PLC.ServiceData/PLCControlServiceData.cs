// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.PLC.ServiceData
{
    [Serializable]
    public class PLCControlServiceData
    {
        public string PortSignName { set; get; }

        public int DeviceNumber { set; get; }

        public int[] Number { set; get; }

        public ControlPLCType ControlPLCType { set; get; }

        public int QueryTimeForAutoUpload { set; get; }

        /// <summary>
        /// True NO, False NC
        /// </summary>
        public bool RelayStatus { set; get; }
    }
}