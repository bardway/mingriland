// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;

using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.Projector.View.Config
{
    public class ProjectorDeviceSettings : DeviceSettings
    {
        public List<ProjectorSerialPortSettings> SerialPortSettings { get; } = new List<ProjectorSerialPortSettings>();

        public void RemoveSerialPortSettings(string portName)
        {
            var serialPortSettings = SerialPortSettings.Find(s => s.PortName == portName);
            if (serialPortSettings != null)
            {
                SerialPortSettings.Remove(serialPortSettings);
            }
        }

        public static ProjectorDeviceSettings GetProjectorDeviceSettings()
        {
            return (ProjectorDeviceSettings)ProjectorDeviceService.GetProjectorDeviceService().DeviceSettings;
        }

        public static ProjectorSerialPortSettings GetSerialPortSettings(string portName)
        {
            var projectorDeviceSettings = GetProjectorDeviceSettings();
            var deviceSettings = projectorDeviceSettings.SerialPortSettings.Find(s => s.PortName == portName);
            return deviceSettings;
        }

        public static ProjectorSerialPortSettings GetSerialPortSettingsFromDetailDevice(string portName, int deviceNumber)
        {
            var projectorDeviceSettings = GetProjectorDeviceSettings();
            return (from projectorSerialPortSettingse in projectorDeviceSettings.SerialPortSettings
                    where projectorSerialPortSettingse.PortName == portName
                    from projectorDetailDeviceSettingse in projectorSerialPortSettingse.ProjectorDetailDevicesSettings
                    where projectorDetailDeviceSettingse.DeviceNumber == deviceNumber
                    select projectorSerialPortSettingse).FirstOrDefault();
        }

        public static SerialPortInfo GetSerialPortInfo(string portName)
        {
            var serialPortSettings = GetSerialPortSettings(portName);
            return serialPortSettings?.GetSerialPortInfo();
        }

        public static ProjectorDetailDeviceSettings GetProjectorDetailDeviceSettings(string portName, int deviceNumber)
        {
            var projectorDeviceSettings = GetProjectorDeviceSettings();
            return projectorDeviceSettings.SerialPortSettings.Where(projectorSerialPortSettingse => projectorSerialPortSettingse.PortName == portName).SelectMany(projectorSerialPortSettingse => projectorSerialPortSettingse.ProjectorDetailDevicesSettings).FirstOrDefault(projectorDetailDeviceSettingse => projectorDetailDeviceSettingse.DeviceNumber == deviceNumber);
        }
    }
}