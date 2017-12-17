// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.NavFrame;
using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.Projector.ServiceData;
using TLAuto.Device.Projector.View.Config;
using TLAuto.Device.Projector.View.NavPages;
using TLAuto.Log;
#endregion

namespace TLAuto.Device.Projector.View.Models
{
    public class ProjectorDeviceInfo : ViewModelBase
    {
        private const string LogModuleName = "Projector";
        private readonly LogWraper _logWraper = new LogWraper(LogModuleName);

        private int _deviceNumber;

        private string _headerName;
        private string _portName;

        public ProjectorDeviceInfo(ProjectorInfo projectorInfo, string portName)
        {
            ProjectorDeviceType = projectorInfo.ProjectorDeviceType;
            HeaderName = projectorInfo.ProjectorHeaderName;
            DeviceNumber = projectorInfo.DeviceNumber;
            UpdatePortName(portName);
            InitEditCommand();
            InitPowerOnCommand();
            InitPowerOffCommand();
            InitOpenLogFileCommand();
        }

        public ProjectorDeviceType ProjectorDeviceType { get; }

        public string HeaderName
        {
            set
            {
                _headerName = value;
                RaisePropertyChanged();
            }
            get => _headerName;
        }

        public int DeviceNumber
        {
            set
            {
                _deviceNumber = value;
                RaisePropertyChanged();
            }
            get => _deviceNumber;
        }

        public string LogMsgId { get; } = Guid.NewGuid().ToString();

        internal ProjectorSerialPortInfo Parent => ProjectorDeviceService.GetProjectorSerialPortInfo(_portName);

        public override void Cleanup()
        {
            var serialPortSettings = ProjectorDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
            serialPortSettings.RemoveDetailDevice(HeaderName);
            base.Cleanup();
        }

        public async Task<bool> PowerOn()
        {
            var tlProjectorDevice = new TLAutoProjectorDevice(Parent.TLSerialPort, ProjectorDeviceType, LogModuleName);
            return await tlProjectorDevice.TLAutoProjectorDeviceSerialPort.PowerOn();
        }

        public async Task<bool> PowerOff()
        {
            var tlProjectorDevice = new TLAutoProjectorDevice(Parent.TLSerialPort, ProjectorDeviceType, LogModuleName);
            return await tlProjectorDevice.TLAutoProjectorDeviceSerialPort.PowerOff();
        }

        public void UpdatePortName(string portName)
        {
            _portName = portName;
        }

        private void WriteMsgWithLog(string msg, StatusNotificationType statusType)
        {
            Messenger.Default.Send(new StatusNotificationMessage(msg, statusType), LogMsgId);
            var logMsg = $"投影仪_{HeaderName}_{msg}";
            switch (statusType)
            {
                case StatusNotificationType.RInfo:
                    _logWraper.Info(logMsg);
                    break;
                case StatusNotificationType.NInfo:
                    _logWraper.Info(logMsg);
                    break;
                case StatusNotificationType.Error:
                    _logWraper.Error(logMsg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null);
            }
        }

        private async Task SendPowerCommand(bool onOrOff)
        {
            var serialPortSettings = ProjectorDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
            var serviceData = new ProjectorControlServiceData
                              {
                                  PortSignName = serialPortSettings.PortSignName,
                                  DeviceNumber = DeviceNumber,
                                  PowerOnOrOff = onOrOff
                              };
            var signName = onOrOff ? "开机" : "关机";
            var resultInfo = await SendWcfCommandHelper.Send(serviceData);
            if (resultInfo != null)
            {
                var msgInfo = resultInfo.IsError ? resultInfo.ErrorMsg : (resultInfo.Data[0].ToBoolean() ? $"{signName}成功。" : $"{signName}失败");
                var statusType = resultInfo.IsError ? StatusNotificationType.Error : (resultInfo.Data[0].ToBoolean() ? StatusNotificationType.RInfo : StatusNotificationType.NInfo);
                WriteMsgWithLog(msgInfo, statusType);
            }
            else
            {
                WriteMsgWithLog($"{signName}{SendWcfCommandHelper.ErrorInfoForCommandTimeOutOrException}", StatusNotificationType.Error);
            }
        }

        #region Event Mvvmbindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var detailDeviceSettings = ProjectorDeviceSettings.GetProjectorDetailDeviceSettings(_portName, DeviceNumber);
                                               var projectorSettingsInfo = new ProjectorSettingsInfo(true, detailDeviceSettings.GetProjectorInfo());
                                               var navFrameInfos = new List<NavFrameInfo>
                                                                   {
                                                                       new NavFrameInfo(projectorSettingsInfo)
                                                                   };
                                               var nav = new NavFrameWindow(navFrameInfos, "编辑投影仪设备信息");
                                               if (nav.ShowDialog() == true)
                                               {
                                                   detailDeviceSettings.HeaderName = projectorSettingsInfo.Current.ProjectorHeaderName;
                                                   detailDeviceSettings.DeviceNumber = projectorSettingsInfo.Current.DeviceNumber;
                                                   if (ProjectorDeviceService.SaveSettings())
                                                   {
                                                       HeaderName = projectorSettingsInfo.Current.ProjectorHeaderName;
                                                       DeviceNumber = projectorSettingsInfo.Current.DeviceNumber;
                                                   }
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

        private void InitPowerOnCommand()
        {
            PowerOnCommand = new RelayCommand(async () => { await SendPowerCommand(true); });
        }

        public RelayCommand PowerOnCommand { private set; get; }

        private void InitPowerOffCommand()
        {
            PowerOffCommand = new RelayCommand(async () => { await SendPowerCommand(false); });
        }

        public RelayCommand PowerOffCommand { private set; get; }

        private void InitOpenLogFileCommand()
        {
            OpenLogFileCommand = new RelayCommand(() =>
                                                  {
                                                      var logFilePath = Path.Combine(Logger.RootLogPath, LogModuleName + ".log");
                                                      try
                                                      {
                                                          Process.Start(logFilePath);
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          MessageBox.Show(ex.Message);
                                                      }
                                                  });
        }

        public RelayCommand OpenLogFileCommand { private set; get; }
        #endregion
    }
}