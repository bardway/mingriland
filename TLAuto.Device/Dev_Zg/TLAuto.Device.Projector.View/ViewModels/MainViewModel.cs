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
using TLAuto.Device.Projector.View.Config;
using TLAuto.Device.Projector.View.Models;
using TLAuto.Device.Projector.View.NavPages;
#endregion

namespace TLAuto.Device.Projector.View.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoaded;

        public MainViewModel()
        {
            InitAddSerialPortCommand();
            InitLoadedCommand();
            InitRemoveSerialPortCommand();
        }

        public ObservableCollection<ProjectorSerialPortInfo> ProjectorSerialPortInfos { get; } = new ObservableCollection<ProjectorSerialPortInfo>();

        public override void Cleanup()
        {
            foreach (var projectorSerialPortInfo in ProjectorSerialPortInfos)
            {
                projectorSerialPortInfo.Cleanup();
            }
            ProjectorSerialPortInfos.Clear();
            base.Cleanup();
        }

        private void AddNewSerialPortInfoWithSave(SerialPortSettingsInfo serialPortSettingsInfo, ProjectorSettingsInfo projectorSettingsInfo)
        {
            var projectorSerialPortInfo = new ProjectorSerialPortInfo(serialPortSettingsInfo.Current.Item1);
            ProjectorSerialPortInfos.Add(projectorSerialPortInfo);
            var deviceSettings = ProjectorDeviceSettings.GetProjectorDeviceSettings();
            deviceSettings.SerialPortSettings.Add(new ProjectorSerialPortSettings(serialPortSettingsInfo.Current.Item1));
            if (ProjectorDeviceService.SaveSettings())
            {
                projectorSerialPortInfo.AddProjectorDeviceInfoWithSave(projectorSettingsInfo.Current);
            }
        }

        internal List<Tuple<SerialPortInfo, bool>> GetAllUsedSerialPortInfo()
        {
            return ProjectorSerialPortInfos.Select(projectorSerialPortInfo => projectorSerialPortInfo.GetCurrentUsedSerialPortInfo()).ToList();
        }

        #region Events Mvvmbindings
        private void InitAddSerialPortCommand()
        {
            AddSerialPortCommand = new RelayCommand(() =>
                                                    {
                                                        var serialPortSettingsInfo = new SerialPortSettingsInfo(GetAllUsedSerialPortInfo());
                                                        var projectorSettingsInfo = new ProjectorSettingsInfo();
                                                        var navFrameInfos = new List<NavFrameInfo>
                                                                            {
                                                                                new NavFrameInfo(serialPortSettingsInfo),
                                                                                new NavFrameInfo(projectorSettingsInfo)
                                                                            };
                                                        var nav = new NavFrameWindow(navFrameInfos, "添加串口信息");
                                                        if (nav.ShowDialog() == true)
                                                        {
                                                            var projectorSerialPortInfo = ProjectorSerialPortInfos.FirstOrDefault(s => s.PortName == serialPortSettingsInfo.Current.Item1.PortName);
                                                            var existsPortName = projectorSerialPortInfo != null;
                                                            if (!existsPortName)
                                                            {
                                                                AddNewSerialPortInfoWithSave(serialPortSettingsInfo, projectorSettingsInfo);
                                                            }
                                                            else
                                                            {
                                                                projectorSerialPortInfo.AddProjectorDeviceInfoWithSave(projectorSettingsInfo.Current);
                                                            }
                                                        }
                                                    });
        }

        public RelayCommand AddSerialPortCommand { private set; get; }

        private void InitRemoveSerialPortCommand()
        {
            RemoveSerialPortCommand = new RelayCommand(() =>
                                                       {
                                                           var dialog = new RemoveItemsView("删除通信串口", ProjectorSerialPortInfos.Select(s => s.PortName).ToList());
                                                           if (dialog.ShowDialog() == true)
                                                           {
                                                               var removePortNames = dialog.SelectedItems;
                                                               foreach (var portName in removePortNames)
                                                               {
                                                                   var serialPortInfo = ProjectorSerialPortInfos.FirstOrDefault(s => s.PortName == portName);
                                                                   if (serialPortInfo != null)
                                                                   {
                                                                       serialPortInfo.Cleanup();
                                                                       ProjectorSerialPortInfos.Remove(serialPortInfo);
                                                                   }
                                                               }
                                                               ProjectorDeviceService.SaveSettings();
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
                                                 var deviceSettings = ProjectorDeviceSettings.GetProjectorDeviceSettings();
                                                 foreach (var projectorSerialPortSettings in deviceSettings.SerialPortSettings)
                                                 {
                                                     var projectorSerialPortInfo = new ProjectorSerialPortInfo(projectorSerialPortSettings.GetSerialPortInfo());
                                                     foreach (var projectorDetailDeviceSettingse in projectorSerialPortSettings.ProjectorDetailDevicesSettings)
                                                     {
                                                         projectorSerialPortInfo.ProjectorInfos.Add(new ProjectorDeviceInfo(projectorDetailDeviceSettingse.GetProjectorInfo(), projectorSerialPortSettings.PortName));
                                                     }
                                                     ProjectorSerialPortInfos.Add(projectorSerialPortInfo);
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}