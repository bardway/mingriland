// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.IO.Ports;

using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.Projector.View.NavPages;
#endregion

namespace TLAuto.Device.Projector.View.Config
{
    public class ProjectorSerialPortSettings
    {
        public ProjectorSerialPortSettings() { }

        public ProjectorSerialPortSettings(SerialPortInfo serialPortInfo)
        {
            UpdateInfo(serialPortInfo);
        }

        public string PortSignName { set; get; }

        public string PortName { set; get; }

        public int BaudRates { set; get; }

        public int DataBits { set; get; }

        public Parity Parity { set; get; }

        public StopBits StopBits { set; get; }

        public List<ProjectorDetailDeviceSettings> ProjectorDetailDevicesSettings { get; } = new List<ProjectorDetailDeviceSettings>();

        public void AddDetailDevice(ProjectorInfo projectorInfo)
        {
            ProjectorDetailDevicesSettings.Add(new ProjectorDetailDeviceSettings(projectorInfo));
        }

        public void RemoveDetailDevice(string headerName)
        {
            var projectorDetailDeviceSettings = ProjectorDetailDevicesSettings.Find(s => s.HeaderName == headerName);
            if (projectorDetailDeviceSettings != null)
            {
                ProjectorDetailDevicesSettings.Remove(projectorDetailDeviceSettings);
            }
        }

        public void UpdateInfo(SerialPortInfo serialPortInfo)
        {
            PortSignName = serialPortInfo.PortSignName;
            PortName = serialPortInfo.PortName;
            BaudRates = serialPortInfo.BaudRates;
            DataBits = serialPortInfo.DataBits;
            Parity = serialPortInfo.Parity;
            StopBits = serialPortInfo.StopBits;
        }

        public SerialPortInfo GetSerialPortInfo()
        {
            var serialPortInfo = new SerialPortInfo
                                 {
                                     PortSignName = PortSignName,
                                     PortName = PortName,
                                     BaudRates = BaudRates,
                                     DataBits = DataBits,
                                     Parity = Parity,
                                     StopBits = StopBits
                                 };
            return serialPortInfo;
        }
    }
}