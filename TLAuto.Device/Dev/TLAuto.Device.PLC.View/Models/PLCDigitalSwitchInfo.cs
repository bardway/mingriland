// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight.Command;
#endregion

namespace TLAuto.Device.PLC.View.Models
{
    public class PLCDigitalSwitchInfo
    {
        public PLCDigitalSwitchInfo(string portName, int deviceNumber, int controlNumber)
        {
            PortName = portName;
            DeviceNumber = deviceNumber;
            ControlNumber = controlNumber;
            InitMouseLeftButtonDownCommand();
        }

        public RelayCommand MouseLeftButtonDownCommand { private set; get; }

        public string PortName { get; }

        public int DeviceNumber { get; }

        public int ControlNumber { get; }

        private void InitMouseLeftButtonDownCommand()
        {
            MouseLeftButtonDownCommand = new RelayCommand(async () =>
                                                          {
                                                              var plcDeviceInfo = PLCDeviceService.GetPLCDeviceInfo(PortName, DeviceNumber);
                                                              await plcDeviceInfo.RaiseDigitalSwitchStatus(ControlNumber);
                                                          });
        }
    }
}