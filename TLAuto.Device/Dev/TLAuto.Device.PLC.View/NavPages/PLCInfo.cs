// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.PLC.View.NavPages
{
    public class PLCInfo
    {
        public PLCDeviceType PLCDeviceType { set; get; }

        public bool HasControlRelayResultStatus { set; get; }

        public string PLCHeaderName { set; get; }

        public int DeviceNumber { set; get; }

        public int DigitalSwitchNumber { set; get; }

        public int RelayNumber { set; get; }
    }
}