// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.Controls.NavFrame;
using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.PLC.View.Config;
using TLAuto.Device.PLC.View.Models;
using TLAuto.Device.PLC.View.NavPages;
#endregion

namespace TLAuto.Device.PLC.View.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoaded;

        public MainViewModel()
        {
            InitAddSerialPortCommand();
            InitRemoveSerialPortCommand();
            InitLoadedCommand();
        }

        public ObservableCollection<PLCSerialPortInfo> PLCSerialPortInfos { get; } = new ObservableCollection<PLCSerialPortInfo>();

        public override void Cleanup()
        {
            foreach (var plcSerialPortInfo in PLCSerialPortInfos)
            {
                plcSerialPortInfo.Cleanup();
            }
            PLCSerialPortInfos.Clear();
            base.Cleanup();
        }

        private void AddNewSerialPortInfoWithSave(SerialPortSettingsInfo serialPortSettingsInfo, PLCSettingsInfo plcSettingsInfo)
        {
            var plcSerialPortInfo = new PLCSerialPortInfo(serialPortSettingsInfo.Current.Item1);
            PLCSerialPortInfos.Add(plcSerialPortInfo);
            var deviceSettings = PLCDeviceSettings.GetPLCDeviceSettings();
            deviceSettings.SerialPortSettings.Add(new PLCSerialPortSettings(serialPortSettingsInfo.Current.Item1));
            if (PLCDeviceService.SaveSettings())
            {
                plcSerialPortInfo.AddPLCDeviceInfoWithSave(plcSettingsInfo.Current);
            }
        }

        internal List<Tuple<SerialPortInfo, bool>> GetAllUsedSerialPortInfo()
        {
            return PLCSerialPortInfos.Select(plcSerialPortInfo => plcSerialPortInfo.GetCurrentUsedSerialPortInfo()).ToList();
        }

        #region Event Mvvmbindings
        private void InitAddSerialPortCommand()
        {
            AddSerialPortCommand = new RelayCommand(() =>
                                                    {
                                                        var serialPortSettingsInfo = new SerialPortSettingsInfo();
                                                        var plcSettingsInfo = new PLCSettingsInfo();
                                                        var navFrameInfos = new List<NavFrameInfo>
                                                                            {
                                                                                new NavFrameInfo(serialPortSettingsInfo),
                                                                                new NavFrameInfo(plcSettingsInfo)
                                                                            };
                                                        var nav = new NavFrameWindow(navFrameInfos, "添加串口信息");
                                                        if (nav.ShowDialog() == true)
                                                        {
                                                            var plcSerialPortInfo = PLCSerialPortInfos.FirstOrDefault(s => s.PortName == serialPortSettingsInfo.Current.Item1.PortName);
                                                            var existsPortName = plcSerialPortInfo != null;
                                                            if (!existsPortName)
                                                            {
                                                                AddNewSerialPortInfoWithSave(serialPortSettingsInfo, plcSettingsInfo);
                                                            }
                                                            else
                                                            {
                                                                plcSerialPortInfo.AddPLCDeviceInfoWithSave(plcSettingsInfo.Current);
                                                            }
                                                        }
                                                    });
        }

        public RelayCommand AddSerialPortCommand { private set; get; }

        private void InitRemoveSerialPortCommand()
        {
            RemoveSerialPortCommand = new RelayCommand(() =>
                                                       {
                                                           var dialog = new RemoveItemsView("删除通信串口", PLCSerialPortInfos.Select(s => s.PortName).ToList());
                                                           if (dialog.ShowDialog() == true)
                                                           {
                                                               var removePortNames = dialog.SelectedItems;
                                                               foreach (var portName in removePortNames)
                                                               {
                                                                   var serialPortInfo = PLCSerialPortInfos.FirstOrDefault(s => s.PortName == portName);
                                                                   if (serialPortInfo != null)
                                                                   {
                                                                       serialPortInfo.Cleanup();
                                                                       PLCSerialPortInfos.Remove(serialPortInfo);
                                                                   }
                                                               }
                                                               PLCDeviceService.SaveSettings();
                                                           }
                                                       });
        }

        public RelayCommand RemoveSerialPortCommand { private set; get; }

        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(() =>
                                             {
                                                 if (_isLoaded)
                                                 {
                                                     return;
                                                 }
                                                 _isLoaded = true;
                                                 var deviceSettings = PLCDeviceSettings.GetPLCDeviceSettings();
                                                 foreach (var projectorSerialPortSettings in deviceSettings.SerialPortSettings)
                                                 {
                                                     var projectorSerialPortInfo = new PLCSerialPortInfo(projectorSerialPortSettings.GetSerialPortInfo());
                                                     foreach (var projectorDetailDeviceSettingse in projectorSerialPortSettings.PLCDetailDevicesSettings)
                                                     {
                                                         projectorSerialPortInfo.PLCInfos.Add(new PLCDeviceInfo(projectorDetailDeviceSettingse.GetPLCInfo(), projectorSerialPortSettings.PortName));
                                                     }
                                                     PLCSerialPortInfos.Add(projectorSerialPortInfo);
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}