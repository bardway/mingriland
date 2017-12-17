// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.IO.Ports;

using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.PLC.View.NavPages;
#endregion

namespace TLAuto.Device.PLC.View.Config
{
    public class PLCSerialPortSettings
    {
        public PLCSerialPortSettings() { }

        public PLCSerialPortSettings(SerialPortInfo serialPortInfo)
        {
            UpdateInfo(serialPortInfo);
        }

        public string PortSignName { set; get; }

        public string PortName { set; get; }

        public int BaudRates { set; get; }

        public int DataBits { set; get; }

        public Parity Parity { set; get; }

        public StopBits StopBits { set; get; }

        public List<PLCDetailDeviceSettings> PLCDetailDevicesSettings { get; } = new List<PLCDetailDeviceSettings>();

        public void AddDetailDevice(PLCInfo plcInfo)
        {
            PLCDetailDevicesSettings.Add(new PLCDetailDeviceSettings(plcInfo));
        }

        public void RemoveDetailDevice(string headerName)
        {
            var projectorDetailDeviceSettings = PLCDetailDevicesSettings.Find(s => s.HeaderName == headerName);
            if (projectorDetailDeviceSettings != null)
            {
                PLCDetailDevicesSettings.Remove(projectorDetailDeviceSettings);
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