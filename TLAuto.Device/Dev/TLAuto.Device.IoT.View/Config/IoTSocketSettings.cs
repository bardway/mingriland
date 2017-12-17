// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using TLAuto.Device.IoT.View.Models.Enums;
#endregion

namespace TLAuto.Device.IoT.View.Config
{
    public class IoTSocketSettings
    {
        public IoTSocketSettings() { }

        public IoTSocketSettings(string ip, int port, string signName, IoTSocketType iotSocketType)
        {
            UpdateInfo(ip, port, signName, iotSocketType);
        }

        public string Ip { set; get; }

        public int Port { set; get; }

        public string SignName { get; set; }

        public IoTSocketType IotSocketType { get; set; }

        public List<IoTDetailDeviceSettings> IoTDetailDevicesSettings { get; } = new List<IoTDetailDeviceSettings>();

        public void UpdateInfo(string ip, int port, string signName, IoTSocketType iotSocketType)
        {
            Ip = ip;
            Port = port;
            SignName = signName;
            IotSocketType = iotSocketType;
        }

        public void AddDetailDevice(int deviceNumber, string deviceHeader)
        {
            IoTDetailDevicesSettings.Add(new IoTDetailDeviceSettings(deviceNumber, deviceHeader));
        }

        public void RemoveDetailDevice(string deviceHeader)
        {
            var ioTDetailDeviceSettings = IoTDetailDevicesSettings.Find(s => s.DeviceHeader == deviceHeader);
            if (ioTDetailDeviceSettings != null)
            {
                IoTDetailDevicesSettings.Remove(ioTDetailDeviceSettings);
            }
        }
    }
}