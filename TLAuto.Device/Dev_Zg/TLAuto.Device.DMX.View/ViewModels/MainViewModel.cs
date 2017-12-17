// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.DMX.View.Config;
using TLAuto.Device.DMX.View.Models;
using TLAuto.Device.DMX.View.Views;
#endregion

namespace TLAuto.Device.DMX.View.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoaded;

        private bool _isLoadedDMXDevice;

        public MainViewModel()
        {
            InitAddDMXDeviceCommand();
            InitRemoveDMXDeviceCommand();
            InitLoadedCommand();
            InitLoadDMXDeviceCommand();
        }

        public bool IsLoadedDMXDevice
        {
            set
            {
                _isLoadedDMXDevice = value;
                RaisePropertyChanged();
            }
            get => _isLoadedDMXDevice;
        }

        public TLAutoDMXDevice TLDMXDevice { get; } = new TLAutoDMXDevice();

        public ObservableCollection<DMXDeviceInfo> DeviceInfos { get; } = new ObservableCollection<DMXDeviceInfo>();

        public override void Cleanup()
        {
            foreach (var deviceInfo in DeviceInfos)
            {
                deviceInfo.Cleanup();
            }
            DeviceInfos.Clear();
            try
            {
#pragma warning disable 4014
                TLDMXDevice.CloseDMXDevice();
#pragma warning restore 4014
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.Cleanup();
        }

        public async Task ControlMulitiSingleChannel(IEnumerable<Tuple<int, int>> multiChannels)
        {
            await TLDMXDevice.SendMulitiSingleChannel(multiChannels);
        }

        #region Event Mvvmbindings
        private void InitAddDMXDeviceCommand()
        {
            AddDMXDeviceCommand = new RelayCommand(() =>
                                                   {
                                                       var dialog = new AddDMXDeviceView();
                                                       if (dialog.ShowDialog() == true)
                                                       {
                                                           var dmxDeviceName = dialog.DMXDeviceName;
                                                           var channelBegin = dialog.ChannelBegin;
                                                           var channelEnd = dialog.ChannelEnd;
                                                           var dmxDeviceInfo = new DMXDeviceInfo(dmxDeviceName, channelBegin, channelEnd);
                                                           DeviceInfos.Add(dmxDeviceInfo);
                                                           var deviceSettings = DMXDeviceSettings.GetDMXDeviceSettings();
                                                           deviceSettings.DeviceInfosSettings.Add(new DMXDeviceInfoSettings(dmxDeviceName, channelBegin, channelEnd));
                                                           DMXDeviceService.SaveSettings();
                                                       }
                                                   });
        }

        public RelayCommand AddDMXDeviceCommand { private set; get; }

        private void InitRemoveDMXDeviceCommand()
        {
            RemoveDMXDeviceCommand = new RelayCommand(() =>
                                                      {
                                                          var dialog = new RemoveItemsView("删除DMX设备", DeviceInfos.Select(s => s.HeaderName).ToList());
                                                          if (dialog.ShowDialog() == true)
                                                          {
                                                              var removeHeaderNames = dialog.SelectedItems;
                                                              foreach (var name in removeHeaderNames)
                                                              {
                                                                  var deviceInfo = DeviceInfos.FirstOrDefault(s => s.HeaderName == name);
                                                                  if (deviceInfo != null)
                                                                  {
                                                                      deviceInfo.Cleanup();
                                                                      DeviceInfos.Remove(deviceInfo);
                                                                  }
                                                              }
                                                              DMXDeviceService.SaveSettings();
                                                          }
                                                      });
        }

        public RelayCommand RemoveDMXDeviceCommand { private set; get; }

        private void InitLoadDMXDeviceCommand()
        {
            LoadDMXDeviceCommand = new RelayCommand(async () =>
                                                    {
                                                        try
                                                        {
                                                            if (IsLoadedDMXDevice)
                                                            {
                                                                await TLDMXDevice.CloseDMXDevice();
                                                            }
                                                            else
                                                            {
                                                                await TLDMXDevice.LoadDMXDevice();
                                                            }
                                                            IsLoadedDMXDevice = TLDMXDevice.IsLoaded;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            MessageBox.Show(ex.Message);
                                                        }
                                                    });
        }

        public RelayCommand LoadDMXDeviceCommand { private set; get; }

        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(() =>
                                             {
                                                 if (_isLoaded)
                                                 {
                                                     return;
                                                 }
                                                 _isLoaded = true;
                                                 var deviceSettings = DMXDeviceSettings.GetDMXDeviceSettings();
                                                 foreach (var dmxDeviceInfoSettings in deviceSettings.DeviceInfosSettings)
                                                 {
                                                     var dmxDeviceInfo = new DMXDeviceInfo(dmxDeviceInfoSettings.HeaderName, dmxDeviceInfoSettings.ChannelBegin, dmxDeviceInfoSettings.ChannelEnd);
                                                     for (var i = 0; i < dmxDeviceInfo.ChannelInfos.Count; i++)
                                                     {
                                                         dmxDeviceInfo.ChannelInfos[i].Remarks = dmxDeviceInfoSettings.ChannelInfosSettings[i].Remarks;
                                                     }
                                                     DeviceInfos.Add(dmxDeviceInfo);
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}