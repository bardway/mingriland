// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;

using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.PLC.View.Config
{
    public class PLCDeviceSettings : DeviceSettings
    {
        public List<PLCSerialPortSettings> SerialPortSettings { get; } = new List<PLCSerialPortSettings>();

        public void RemoveSerialPortSettings(string portName)
        {
            var serialPortSettings = SerialPortSettings.Find(s => s.PortName == portName);
            if (serialPortSettings != null)
            {
                SerialPortSettings.Remove(serialPortSettings);
            }
        }

        public static PLCDeviceSettings GetPLCDeviceSettings()
        {
            return (PLCDeviceSettings)PLCDeviceService.GetPLCDeviceService().DeviceSettings;
        }

        public static PLCSerialPortSettings GetSerialPortSettings(string portName)
        {
            var plcDeviceSettings = GetPLCDeviceSettings();
            var deviceSettings = plcDeviceSettings.SerialPortSettings.Find(s => s.PortName == portName);
            return deviceSettings;
        }

        public static PLCSerialPortSettings GetSerialPortSettingsFromDetailDevice(string portName, int deviceNumber)
        {
            var plcDeviceSettings = GetPLCDeviceSettings();
            return (from plcSerialPortSettingse in plcDeviceSettings.SerialPortSettings
                    where plcSerialPortSettingse.PortName == portName
                    from plcDetailDeviceSettingse in plcSerialPortSettingse.PLCDetailDevicesSettings
                    where plcDetailDeviceSettingse.DeviceNumber == deviceNumber
                    select plcSerialPortSettingse).FirstOrDefault();
        }

        public static SerialPortInfo GetSerialPortInfo(string portName)
        {
            var serialPortSettings = GetSerialPortSettings(portName);
            return serialPortSettings?.GetSerialPortInfo();
        }

        public static PLCDetailDeviceSettings GetPLCDetailDeviceSettings(string portName, int deviceNumber)
        {
            var plcDeviceSettings = GetPLCDeviceSettings();
            return plcDeviceSettings.SerialPortSettings.Where(plcSerialPortSettingse => plcSerialPortSettingse.PortName == portName).SelectMany(plcSerialPortSettingse => plcSerialPortSettingse.PLCDetailDevicesSettings).FirstOrDefault(plcDetailDevicesSetting => plcDetailDevicesSetting.DeviceNumber == deviceNumber);
        }
    }
}