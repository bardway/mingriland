// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;

using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.IoT.View.Config
{
    public class IoTDeviceSettings : DeviceSettings
    {
        public List<IoTSocketSettings> SocketSettings { get; } = new List<IoTSocketSettings>();

        public void RemoveSocketSettings(string signName)
        {
            var socketSettings = SocketSettings.Find(s => s.SignName == signName);
            if (socketSettings != null)
            {
                SocketSettings.Remove(socketSettings);
            }
        }

        public static IoTDeviceSettings GetIoTDeviceSettings()
        {
            return (IoTDeviceSettings)IoTDeviceService.GetIoTDeviceService().DeviceSettings;
        }

        public static IoTSocketSettings GetIoTDeviceSettings(string signName)
        {
            var ioTDeviceSettings = GetIoTDeviceSettings();
            var deviceSettings = ioTDeviceSettings.SocketSettings.Find(s => s.SignName == signName);
            return deviceSettings;
        }

        public static IoTSocketSettings GetScoketSettingsFromDetailDevice(string signName, int deviceNumber)
        {
            var ioTDeviceSettings = GetIoTDeviceSettings();
            return (from ioTSocketSettings in ioTDeviceSettings.SocketSettings
                    where ioTSocketSettings.SignName == signName
                    from ioTDetailDeviceSettings in ioTSocketSettings.IoTDetailDevicesSettings
                    where ioTDetailDeviceSettings.DeviceNumber == deviceNumber
                    select ioTSocketSettings).FirstOrDefault();
        }

        public static IoTDetailDeviceSettings GetIoTDetailDeviceSettings(string signName, int deviceNumber)
        {
            var ioTDeviceSettings = GetIoTDeviceSettings();
            return ioTDeviceSettings.SocketSettings.Where(ioTSocketSettings => ioTSocketSettings.SignName == signName).SelectMany(ioTSocketSettings => ioTSocketSettings.IoTDetailDevicesSettings).FirstOrDefault(projectorDetailDeviceSettingse => projectorDetailDeviceSettingse.DeviceNumber == deviceNumber);
        }
    }
}