// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.IoT.View.Config
{
    public class IoTDetailDeviceSettings
    {
        public IoTDetailDeviceSettings() { }

        public IoTDetailDeviceSettings(int deviceNumber, string deviceHeader)
        {
            DeviceNumber = deviceNumber;
            DeviceHeader = deviceHeader;
        }

        public int DeviceNumber { set; get; }

        public string DeviceHeader { set; get; }
    }
}