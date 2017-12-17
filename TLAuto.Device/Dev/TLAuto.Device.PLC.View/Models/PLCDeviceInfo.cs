// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.NavFrame;
using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Device.PLC.View.Config;
using TLAuto.Device.PLC.View.NavPages;
using TLAuto.Device.PLC.View.Views;
using TLAuto.Log;
#endregion

namespace TLAuto.Device.PLC.View.Models
{
    public class PLCDeviceInfo : ViewModelBase
    {
        private const string LogModuleName = "PLC";
        internal readonly LogWraper _logWraper = new LogWraper(LogModuleName);

        private int _deviceNumber;

        private int _digitalSwitchNumber;

        private bool _hasControlRelayResultStatus;

        private string _headerName;
        private string _portName;

        private int _relayNumber;

        public PLCDeviceInfo(PLCInfo plcInfo, string portName)
        {
            PLCDeviceType = plcInfo.PLCDeviceType;
            HasControlRelayResultStatus = plcInfo.HasControlRelayResultStatus;
            HeaderName = plcInfo.PLCHeaderName;
            DeviceNumber = plcInfo.DeviceNumber;
            DigitalSwitchNumber = plcInfo.DigitalSwitchNumber;
            RelayNumber = plcInfo.RelayNumber;
            UpdatePortName(portName);
            InitEditCommand();
            InitOpenLogFileCommand();
            InitChangedDeviceNumberCommand();
            InitQuerySwitchStatusCommand();
            InitQueryRelayStatusCommand();
        }

        public PLCDeviceType PLCDeviceType { get; }

        public bool HasControlRelayResultStatus
        {
            set
            {
                _hasControlRelayResultStatus = value;
                RaisePropertyChanged();
            }
            get => _hasControlRelayResultStatus;
        }

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
                ResetControlItem();
                RaisePropertyChanged();
            }
            get => _deviceNumber;
        }

        public int DigitalSwitchNumber
        {
            set
            {
                _digitalSwitchNumber = value;
                ResetControlItem();
                RaisePropertyChanged();
            }
            get => _digitalSwitchNumber;
        }

        public int RelayNumber
        {
            set
            {
                _relayNumber = value;
                ResetControlItem();
                RaisePropertyChanged();
            }
            get => _relayNumber;
        }

        public string LogMsgId { get; } = Guid.NewGuid().ToString();

        public ObservableCollection<PLCDigitalSwitchInfo> DigitalSwitchInfos { get; } = new ObservableCollection<PLCDigitalSwitchInfo>();

        public ObservableCollection<PLCRelayInfo> RelayInfos { get; } = new ObservableCollection<PLCRelayInfo>();

        internal PLCSerialPortInfo Parent => PLCDeviceService.GetPLCSerialPortInfo(_portName);

        public override void Cleanup()
        {
            var serialPortSettings = PLCDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
            serialPortSettings.RemoveDetailDevice(HeaderName);
            base.Cleanup();
        }

        public async Task<byte[]> ControlPLC(PLCControlServiceData serviceData)
        {
            var tlPLCDevice = new TLAutoPLCDevice(Parent.TLSerialPort, HasControlRelayResultStatus, PLCDeviceType, LogModuleName);
            switch (serviceData.ControlPLCType)
            {
                case ControlPLCType.ChangedDeviceNumber:
                {
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.ChangeDeviceNumber(serviceData.DeviceNumber);
                    return new[] {result.ToByte()};
                }
                case ControlPLCType.QueryDigitalSwitch:
                {
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.QueryDigitalSwitchStatus(DeviceNumber, DigitalSwitchNumber);
                    if (result != null)
                    {
                        return result.ToBytes();
                    }
                }
                    break;
                case ControlPLCType.QueryDiaitalSwitchWithAutoUpload:
                {
                    //#if DEBUG
                    //                        var sb = new StringBuilder();
                    //                        foreach (var n in serviceData.Number)
                    //                        {
                    //                            sb.Append(n + ",");
                    //                        }
                    //                        WriteMsgWithLog("监听开关，编号：" + sb, StatusNotificationType.NInfo);
                    //#endif
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.QueryDigitalSwitchStatusWithAutoNotification(DeviceNumber, DigitalSwitchNumber, serviceData.Number, serviceData.QueryTimeForAutoUpload);
                    if (result != null)
                    {
                        return result.ToBytes();
                    }
                }
                    break;
                case ControlPLCType.QueryRelayStatus:
                {
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.QueryRelayStatus(DeviceNumber, RelayNumber);
                    if (result != null)
                    {
                        return result.ToBytes();
                    }
                }
                    break;
                case ControlPLCType.ControlRelay:
                {
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.ControlRelay(DeviceNumber, RelayNumber, serviceData.Number[0], (RelayStatus)Enum.Parse(typeof(RelayStatus), serviceData.RelayStatus.ToInt32().ToString()));
                    return new[] {result.ToByte()};
                }
                case ControlPLCType.SimulationDigitalSwitch:
                {
                    var result = await tlPLCDevice.TLAutoPLCDeviceSerialPort.RaiseDigitalSwitchStatus(DeviceNumber, DigitalSwitchNumber, serviceData.Number[0], SwitchStatus.NC);
                    return new[] {result.ToByte()};
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        internal async Task<bool> RaiseDigitalSwitchStatus(int number)
        {
#if DEBUG
            WriteMsgWithLog("模拟开关触发，编号：" + number, StatusNotificationType.NInfo);
#endif
            var tlPLCDevice = new TLAutoPLCDevice(Parent.TLSerialPort, HasControlRelayResultStatus, PLCDeviceType, LogModuleName);
            return await tlPLCDevice.TLAutoPLCDeviceSerialPort.RaiseDigitalSwitchStatus(DeviceNumber, DigitalSwitchNumber, number, SwitchStatus.NC);
        }

        internal async Task<bool> ControlRelay(int number, bool isNo)
        {
            var serialPortSettings = PLCDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
            var serviceData = new PLCControlServiceData
                              {
                                  PortSignName = serialPortSettings.PortSignName,
                                  ControlPLCType = ControlPLCType.ControlRelay,
                                  DeviceNumber = DeviceNumber,
                                  Number = new[] {number},
                                  RelayStatus = isNo
                              };
            var resultInfo = await SendWcfCommandHelper.Send(serviceData);
            if (resultInfo != null)
            {
                var msgInfo = resultInfo.IsError ? $"查询命令调用失败,原因：{resultInfo.ErrorMsg}" : "";
                if (!msgInfo.IsNullOrEmpty())
                {
                    WriteMsgWithLog(msgInfo, StatusNotificationType.Error);
                }
                else
                {
                    if (resultInfo.Data == null)
                    {
                        WriteMsgWithLog("调用命令超时或出现错误。", StatusNotificationType.Error);
                    }
                    else
                    {
                        var result = resultInfo.Data[0].ToBoolean();
                        if (!result)
                        {
                            WriteMsgWithLog($"继电器编号：{number}控制失败。", StatusNotificationType.Error);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                WriteMsgWithLog(SendWcfCommandHelper.ErrorInfoForQueryFailed, StatusNotificationType.Error);
            }
            return false;
        }

        private void WriteMsgWithLog(string msg, StatusNotificationType statusType)
        {
            Messenger.Default.Send(new StatusNotificationMessage(msg, statusType), LogMsgId);
            var logMsg = $"工控板_{HeaderName}_{msg}";
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

        public void UpdatePortName(string portName)
        {
            _portName = portName;
            ResetControlItem();
        }

        private void ResetControlItem()
        {
            DigitalSwitchInfos.Clear();
            for (var i = 0; i < DigitalSwitchNumber; i++)
            {
                DigitalSwitchInfos.Add(new PLCDigitalSwitchInfo(_portName, DeviceNumber, i + 1));
            }

            RelayInfos.Clear();
            for (var i = 0; i < RelayNumber; i++)
            {
                RelayInfos.Add(new PLCRelayInfo(_portName, DeviceNumber, i + 1));
            }
        }

        #region Event Mvvmbindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var detailDeviceSettings = PLCDeviceSettings.GetPLCDetailDeviceSettings(_portName, DeviceNumber);
                                               var projectorSettingsInfo = new PLCSettingsInfo(true, detailDeviceSettings.GetPLCInfo());
                                               var navFrameInfos = new List<NavFrameInfo>
                                                                   {
                                                                       new NavFrameInfo(projectorSettingsInfo)
                                                                   };
                                               var nav = new NavFrameWindow(navFrameInfos, "编辑工控板设备信息");
                                               if (nav.ShowDialog() == true)
                                               {
                                                   detailDeviceSettings.HeaderName = projectorSettingsInfo.Current.PLCHeaderName;
                                                   detailDeviceSettings.DeviceNumber = projectorSettingsInfo.Current.DeviceNumber;
                                                   detailDeviceSettings.DigitalSwitchNumber = projectorSettingsInfo.Current.DigitalSwitchNumber;
                                                   detailDeviceSettings.RelayNumber = projectorSettingsInfo.Current.RelayNumber;
                                                   if (PLCDeviceService.SaveSettings())
                                                   {
                                                       HeaderName = projectorSettingsInfo.Current.PLCHeaderName;
                                                       DeviceNumber = projectorSettingsInfo.Current.DeviceNumber;
                                                       DigitalSwitchNumber = projectorSettingsInfo.Current.DigitalSwitchNumber;
                                                       RelayNumber = projectorSettingsInfo.Current.RelayNumber;
                                                   }
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

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

        private void InitChangedDeviceNumberCommand()
        {
            ChangedDeviceNumberCommand = new RelayCommand(async () =>
                                                          {
                                                              var serialPortSettings = PLCDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
                                                              var serviceData = new PLCControlServiceData
                                                                                {
                                                                                    PortSignName = serialPortSettings.PortSignName,
                                                                                    ControlPLCType = ControlPLCType.ChangedDeviceNumber,
                                                                                    DeviceNumber = DeviceNumber
                                                                                };
                                                              var resultInfo = await SendWcfCommandHelper.Send(serviceData);
                                                              if (resultInfo != null)
                                                              {
                                                                  var msgInfo = resultInfo.IsError ? resultInfo.ErrorMsg : (resultInfo.Data[0].ToBoolean() ? "更改设备号命令调用成功。" : "更改设备号命令调用失败");
                                                                  var statusType = resultInfo.IsError ? StatusNotificationType.Error : (resultInfo.Data[0].ToBoolean() ? StatusNotificationType.RInfo : StatusNotificationType.NInfo);
                                                                  WriteMsgWithLog(msgInfo, statusType);
                                                              }
                                                              else
                                                              {
                                                                  WriteMsgWithLog("更改设备号命令调用失败", StatusNotificationType.Error);
                                                              }
                                                          });
        }

        public RelayCommand ChangedDeviceNumberCommand { private set; get; }

        private void InitQuerySwitchStatusCommand()
        {
            QuerySwitchStatusCommand = new RelayCommand(async () =>
                                                        {
                                                            var dialog = new QuerySwitchStatusView();
                                                            if (dialog.ShowDialog() == true)
                                                            {
                                                                var switchNums = dialog.SwitchNums;
                                                                var isAutoUpload = dialog.IsAutoUpload;
                                                                var serialPortSettings = PLCDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
                                                                var serviceData = new PLCControlServiceData
                                                                                  {
                                                                                      PortSignName = serialPortSettings.PortSignName,
                                                                                      ControlPLCType = isAutoUpload ? ControlPLCType.QueryDiaitalSwitchWithAutoUpload : ControlPLCType.QueryDigitalSwitch,
                                                                                      DeviceNumber = DeviceNumber,
                                                                                      Number = switchNums,
                                                                                      QueryTimeForAutoUpload = isAutoUpload ? 10000 : 5000
                                                                                  };
                                                                var resultInfo = await SendWcfCommandHelper.Send(serviceData);
                                                                if (resultInfo != null)
                                                                {
                                                                    var msgInfo = resultInfo.IsError ? $"查询命令调用失败,原因：{resultInfo.ErrorMsg}" : "";
                                                                    if (!msgInfo.IsNullOrEmpty())
                                                                    {
                                                                        WriteMsgWithLog(msgInfo, StatusNotificationType.Error);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (resultInfo.Data == null)
                                                                        {
                                                                            WriteMsgWithLog("调用命令超时或出现错误。", StatusNotificationType.Error);
                                                                        }
                                                                        else
                                                                        {
                                                                            var switchItems = resultInfo.Data.ToObject<IEnumerable<SwitchItem>>().ToList();
                                                                            var queryNums = switchNums.ToList();
                                                                            foreach (var switchItem in switchItems)
                                                                            {
                                                                                if (switchItem != null)
                                                                                {
                                                                                    queryNums.Remove(switchItem.SwitchNumber);
                                                                                    WriteMsgWithLog($"开关编号：{switchItem.SwitchNumber},状态：{switchItem.SwitchStatus.GetEnumDescriptionAttribute()}", StatusNotificationType.NInfo);
                                                                                }
                                                                            }
                                                                            if (queryNums.Count > 0)
                                                                            {
                                                                                foreach (var switchNumber in queryNums)
                                                                                {
                                                                                    WriteMsgWithLog($"开关编号：{switchNumber} 收到错误信息，可能原因是服务器端的内部操作超时或出现错误。", StatusNotificationType.NInfo);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    WriteMsgWithLog(SendWcfCommandHelper.ErrorInfoForQueryFailed, StatusNotificationType.Error);
                                                                }
                                                            }
                                                        });
        }

        public RelayCommand QuerySwitchStatusCommand { private set; get; }

        private void InitQueryRelayStatusCommand()
        {
            QueryRelayStatusCommand = new RelayCommand(async () =>
                                                       {
                                                           var serialPortSettings = PLCDeviceSettings.GetSerialPortSettingsFromDetailDevice(_portName, DeviceNumber);
                                                           var serviceData = new PLCControlServiceData
                                                                             {
                                                                                 PortSignName = serialPortSettings.PortSignName,
                                                                                 ControlPLCType = ControlPLCType.QueryRelayStatus,
                                                                                 DeviceNumber = DeviceNumber
                                                                             };
                                                           var resultInfo = await SendWcfCommandHelper.Send(serviceData);
                                                           if (resultInfo != null)
                                                           {
                                                               var msgInfo = resultInfo.IsError ? $"查询命令调用失败,原因：{resultInfo.ErrorMsg}" : "";
                                                               if (!msgInfo.IsNullOrEmpty())
                                                               {
                                                                   WriteMsgWithLog(msgInfo, StatusNotificationType.Error);
                                                               }
                                                               else
                                                               {
                                                                   if (resultInfo.Data == null)
                                                                   {
                                                                       WriteMsgWithLog("调用命令超时或出现错误。", StatusNotificationType.Error);
                                                                   }
                                                                   else
                                                                   {
                                                                       var relayItems = resultInfo.Data.ToObject<IEnumerable<RelayItem>>().ToList();
                                                                       foreach (var relayItem in relayItems)
                                                                       {
                                                                           WriteMsgWithLog($"继电器编号：{relayItem.RelayNumber},状态：{relayItem.RelayStatus.GetEnumDescriptionAttribute()}", StatusNotificationType.NInfo);
                                                                           var relayInfo = RelayInfos.FirstOrDefault(s => s.ControlNumber == relayItem.RelayNumber);
                                                                           if (relayInfo != null)
                                                                           {
                                                                               relayInfo.Status = relayItem.RelayStatus;
                                                                           }
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                           else
                                                           {
                                                               WriteMsgWithLog(SendWcfCommandHelper.ErrorInfoForQueryFailed, StatusNotificationType.Error);
                                                           }
                                                       });
        }

        public RelayCommand QueryRelayStatusCommand { private set; get; }
        #endregion
    }
}