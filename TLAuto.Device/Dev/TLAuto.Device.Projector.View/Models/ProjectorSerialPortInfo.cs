// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.Controls.NavFrame;
using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.Projector.View.Config;
using TLAuto.Device.Projector.View.NavPages;
using TLAuto.Device.Projector.View.ViewModels;
#endregion

namespace TLAuto.Device.Projector.View.Models
{
    public class ProjectorSerialPortInfo : ViewModelBase
    {
        private bool _isOpenSerialPort;

        private string _portName;

        private string _portSignName;

        public ProjectorSerialPortInfo(SerialPortInfo serialPortInfo)
        {
            UpdateInfo(serialPortInfo);
            InitOpenSerialPortCommand();
            InitEditCommand();
            InitDeleteCommand();
        }

        internal TLAutoDeviceSerialPort TLSerialPort { get; } = new TLAutoDeviceSerialPort();

        public string PortName
        {
            set
            {
                _portName = value;
                RaisePropertyChanged();
            }
            get => _portName;
        }

        public string PortSignName
        {
            set
            {
                _portSignName = value;
                RaisePropertyChanged();
            }
            get => _portSignName;
        }

        public bool IsOpenSerialPort
        {
            private set
            {
                _isOpenSerialPort = value;
                RaisePropertyChanged();
            }
            get => _isOpenSerialPort;
        }

        public MainViewModel Parent => ProjectorDeviceService.GetMainViewModel();

        public ObservableCollection<ProjectorDeviceInfo> ProjectorInfos { get; } = new ObservableCollection<ProjectorDeviceInfo>();

        public override void Cleanup()
        {
            foreach (var projectorInfo in ProjectorInfos)
            {
                projectorInfo.Cleanup();
            }
            ProjectorInfos.Clear();
            var deviceSettings = ProjectorDeviceSettings.GetProjectorDeviceSettings();
            deviceSettings.RemoveSerialPortSettings(PortName);
            ClosePort();
            base.Cleanup();
        }

        private void UpdateInfo(SerialPortInfo serialPortInfo)
        {
            PortName = serialPortInfo.PortName;
            PortSignName = serialPortInfo.PortSignName;
            foreach (var projectorDeviceInfo in ProjectorInfos)
            {
                projectorDeviceInfo.UpdatePortName(PortName);
            }
        }

        public void AddProjectorDeviceInfoWithSave(ProjectorInfo projectorInfo)
        {
            ProjectorInfos.Add(new ProjectorDeviceInfo(projectorInfo, PortName));
            var serialPortSettings = ProjectorDeviceSettings.GetSerialPortSettings(PortName);
            serialPortSettings.AddDetailDevice(projectorInfo);
            ProjectorDeviceService.SaveSettings();
        }

        internal Tuple<SerialPortInfo, bool> GetCurrentUsedSerialPortInfo()
        {
            return new Tuple<SerialPortInfo, bool>(ProjectorDeviceSettings.GetSerialPortInfo(PortName), IsOpenSerialPort);
        }

        private void ClosePort()
        {
            try
            {
                if (IsOpenSerialPort)
                {
                    TLSerialPort.Close();
                }
                IsOpenSerialPort = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenPort()
        {
            try
            {
                if (!_isOpenSerialPort)
                {
                    var serialPortInfo = ProjectorDeviceSettings.GetSerialPortInfo(PortName);
                    TLSerialPort.Open(serialPortInfo.PortName,
                                      serialPortInfo.BaudRates,
                                      serialPortInfo.Parity,
                                      serialPortInfo.DataBits,
                                      serialPortInfo.StopBits);
                    IsOpenSerialPort = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Events Mvvmbindings
        private void InitOpenSerialPortCommand()
        {
            OpenSerialPortCommand = new RelayCommand(() =>
                                                     {
                                                         if (IsOpenSerialPort)
                                                         {
                                                             ClosePort();
                                                         }
                                                         else
                                                         {
                                                             OpenPort();
                                                         }
                                                     });
        }

        public RelayCommand OpenSerialPortCommand { private set; get; }

        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var serialPortSettingsInfo = new SerialPortSettingsInfo(Parent.GetAllUsedSerialPortInfo(), true, PortName);
                                               var navFrameInfos = new List<NavFrameInfo>
                                                                   {
                                                                       new NavFrameInfo(serialPortSettingsInfo)
                                                                   };
                                               var nav = new NavFrameWindow(navFrameInfos, "编辑设备信息");
                                               if (nav.ShowDialog() == true)
                                               {
                                                   var oldSerialPortSettings = ProjectorDeviceSettings.GetSerialPortSettings(PortName);
                                                   oldSerialPortSettings.UpdateInfo(serialPortSettingsInfo.Current.Item1);
                                                   UpdateInfo(serialPortSettingsInfo.Current.Item1);
                                                   ProjectorDeviceService.SaveSettings();
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

        private void InitDeleteCommand()
        {
            DeleteCommand = new RelayCommand(() =>
                                             {
                                                 var dialog = new RemoveItemsView("删除设备", ProjectorInfos.Select(s => s.HeaderName).ToList());
                                                 if (dialog.ShowDialog() == true)
                                                 {
                                                     var removeHeaderNames = dialog.SelectedItems;
                                                     foreach (var headerName in removeHeaderNames)
                                                     {
                                                         var removeInfo = ProjectorInfos.FirstOrDefault(s => s.HeaderName == headerName);
                                                         if (removeInfo != null)
                                                         {
                                                             removeInfo.Cleanup();
                                                             ProjectorInfos.Remove(removeInfo);
                                                         }
                                                     }
                                                     ProjectorDeviceService.SaveSettings();
                                                 }
                                             });
        }

        public RelayCommand DeleteCommand { private set; get; }
        #endregion
    }
}