// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.IoT.View.Config;
using TLAuto.Device.IoT.View.Models;
using TLAuto.Device.IoT.View.Models.Enums;
using TLAuto.Device.IoT.View.Views.Dialogs;
#endregion

namespace TLAuto.Device.IoT.View.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoaded;

        public MainViewModel()
        {
            InitAddIpAddressCommand();
            InitLoadedCommand();
            InitRemoveIpAddressCommand();
        }

        public ObservableCollection<IoTSocketInfo> IoTSocketInfos { get; } = new ObservableCollection<IoTSocketInfo>();

        public override void Cleanup()
        {
            foreach (var ioTSocketInfo in IoTSocketInfos)
            {
                ioTSocketInfo.Cleanup();
            }
            IoTSocketInfos.Clear();
            base.Cleanup();
        }

        private void AddNewSocketInfoWithSave(string ip, int port, string signName, int deviceNumber, string deviceHeader, IoTSocketType iotSocketType)
        {
            var iotSocketInfo = new IoTSocketInfo(ip, port, signName, iotSocketType);
            IoTSocketInfos.Add(iotSocketInfo);
            var deviceSettings = IoTDeviceSettings.GetIoTDeviceSettings();
            deviceSettings.SocketSettings.Add(new IoTSocketSettings(ip, port, signName, iotSocketType));
            if (IoTDeviceService.SaveSettings())
            {
                iotSocketInfo.AddIoTDeviceInfoWithSave(deviceNumber, deviceHeader);
            }
        }

        #region Events Mvvmbindings
        private void InitAddIpAddressCommand()
        {
            AddIpAddressCommand = new RelayCommand(() =>
                                                   {
                                                       var editDialog = new EditIpAddressDialog("127.0.0.1", 0, "IoTIp标识符", 0, "IoT设备名", IoTSocketType.Server);
                                                       if (editDialog.ShowDialog() == true)
                                                       {
                                                           var ioTSocketInfo = IoTSocketInfos.FirstOrDefault(s => s.Ip == editDialog.Ip);
                                                           var existsIp = ioTSocketInfo != null;
                                                           if (!existsIp)
                                                           {
                                                               AddNewSocketInfoWithSave(editDialog.Ip, editDialog.Port, editDialog.SignName, editDialog.DeviceNumber, editDialog.DeviceHeader, editDialog.IoTScoketType);
                                                           }
                                                           else
                                                           {
                                                               ioTSocketInfo.AddIoTDeviceInfoWithSave(editDialog.DeviceNumber, editDialog.DeviceHeader);
                                                           }
                                                       }
                                                   });
        }

        public RelayCommand AddIpAddressCommand { private set; get; }

        private void InitRemoveIpAddressCommand()
        {
            RemoveIpAddressCommand = new RelayCommand(() =>
                                                      {
                                                          var dialog = new RemoveItemsView("删除IP地址", IoTSocketInfos.Select(s => s.SignName).ToList());
                                                          if (dialog.ShowDialog() == true)
                                                          {
                                                              var removeSignName = dialog.SelectedItems;
                                                              foreach (var signName in removeSignName)
                                                              {
                                                                  var serialPortInfo = IoTSocketInfos.FirstOrDefault(s => s.SignName == signName);
                                                                  if (serialPortInfo != null)
                                                                  {
                                                                      serialPortInfo.Cleanup();
                                                                      IoTSocketInfos.Remove(serialPortInfo);
                                                                  }
                                                              }
                                                              IoTDeviceService.SaveSettings();
                                                          }
                                                      });
        }

        public RelayCommand RemoveIpAddressCommand { private set; get; }

        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(() =>
                                             {
                                                 if (_isLoaded)
                                                 {
                                                     return;
                                                 }
                                                 _isLoaded = true;
                                                 var deviceSettings = IoTDeviceSettings.GetIoTDeviceSettings();
                                                 foreach (var ioTSocketSettings in deviceSettings.SocketSettings)
                                                 {
                                                     var ioTSocketInfo = new IoTSocketInfo(ioTSocketSettings.Ip, ioTSocketSettings.Port, ioTSocketSettings.SignName, IoTSocketType.Server);
                                                     foreach (var ioTDetailDeviceSettings in ioTSocketSettings.IoTDetailDevicesSettings)
                                                     {
                                                         ioTSocketInfo.IoTInfos.Add(new IoTDeviceInfo(ioTDetailDeviceSettings.DeviceNumber, ioTDetailDeviceSettings.DeviceHeader, ioTSocketSettings.SignName));
                                                     }
                                                     IoTSocketInfos.Add(ioTSocketInfo);
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}