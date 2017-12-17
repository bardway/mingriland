// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Device.PLC.View.NavPages;
#endregion

namespace TLAuto.Device.PLC.View.Config
{
    public class PLCDetailDeviceSettings
    {
        public PLCDetailDeviceSettings() { }

        public PLCDetailDeviceSettings(PLCInfo plcInfo)
        {
            PLCDeviceType = plcInfo.PLCDeviceType;
            HasControlRelayResultStatus = plcInfo.HasControlRelayResultStatus;
            HeaderName = plcInfo.PLCHeaderName;
            DeviceNumber = plcInfo.DeviceNumber;
            DigitalSwitchNumber = plcInfo.DigitalSwitchNumber;
            RelayNumber = plcInfo.RelayNumber;
        }

        public PLCDeviceType PLCDeviceType { set; get; }

        public bool HasControlRelayResultStatus { set; get; }

        public string HeaderName { set; get; }

        public int DeviceNumber { set; get; }

        public int DigitalSwitchNumber { set; get; }

        public int RelayNumber { set; get; }

        public PLCInfo GetPLCInfo()
        {
            return new PLCInfo
                   {
                       PLCDeviceType = PLCDeviceType,
                       HasControlRelayResultStatus = HasControlRelayResultStatus,
                       PLCHeaderName = HeaderName,
                       DeviceNumber = DeviceNumber,
                       DigitalSwitchNumber = DigitalSwitchNumber,
                       RelayNumber = RelayNumber
                   };
        }
    }
}