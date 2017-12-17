// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.View.Models
{
    public class PLCRelayInfo : ObservableObject
    {
        private RelayStatus _status;

        public PLCRelayInfo(string portName, int deviceNumber, int controlNumber)
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

        public RelayStatus Status
        {
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
            get => _status;
        }

        private void InitMouseLeftButtonDownCommand()
        {
            MouseLeftButtonDownCommand = new RelayCommand(async () =>
                                                          {
                                                              var plcDeviceInfo = PLCDeviceService.GetPLCDeviceInfo(PortName, DeviceNumber);
                                                              var result = await plcDeviceInfo.ControlRelay(ControlNumber, Status == RelayStatus.NC);
                                                              if (result)
                                                              {
                                                                  Status = Status == RelayStatus.NC ? RelayStatus.NO : RelayStatus.NC;
                                                              }
                                                          });
        }
    }
}