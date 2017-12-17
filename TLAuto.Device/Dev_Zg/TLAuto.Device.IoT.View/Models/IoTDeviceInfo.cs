// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.IoT.ServiceData;
using TLAuto.Device.IoT.View.Config;
using TLAuto.Device.IoT.View.Models.Enums;
using TLAuto.Device.IoT.View.Views.Dialogs;
using TLAuto.Log;
#endregion

namespace TLAuto.Device.IoT.View.Models
{
    public class IoTDeviceInfo : ViewModelBase
    {
        private const string LogModuleName = "Ioc";
        internal readonly LogWraper _logWraper = new LogWraper(LogModuleName);

        private string _deviceHeader;

        private int _deviceNumber;

        private string _signName;
        //private TLAutoDevicePushCallbackTest _callback;

        public IoTDeviceInfo(int deviceNumber, string deviceHeader, string signName)
        {
            DeviceNumber = deviceNumber;
            DeviceHeader = deviceHeader;
            UpdatePortName(signName);
            InitEditCommand();
            InitTestUploadCheckedCommand();
            InitTestUploadUncheckedComma();
            InitTestClientConnectedCommand();
            InitOpenLogFileCommand();
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

        public string DeviceHeader
        {
            get => _deviceHeader;
            set
            {
                _deviceHeader = value;
                RaisePropertyChanged();
            }
        }

        public string LogMsgId { get; } = Guid.NewGuid().ToString();

        internal IoTSocketInfo Parent => IoTDeviceService.GetIoTSocketInfo(_signName);

        public bool IsShowServerReceivedInfo { private set; get; }

        public override void Cleanup()
        {
            var scoketSettingsFromDetailDevice = IoTDeviceSettings.GetScoketSettingsFromDetailDevice(_signName, DeviceNumber);
            scoketSettingsFromDetailDevice.RemoveDetailDevice(DeviceHeader);
            base.Cleanup();
        }

        public void UpdatePortName(string signName)
        {
            _signName = signName;
        }

        internal void WriteMsgWithLog(string msg, StatusNotificationType statusType)
        {
            Messenger.Default.Send(new StatusNotificationMessage(msg, statusType), LogMsgId);
            var logMsg = $"IoT{DeviceHeader}_{msg}";
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

        public async Task<byte[]> ControlPLC(IoTControlServiceData serviceData)
        {
            var iotSendAsync = new IoTSendStringAsyncSocket(Parent.SocketTcpWrapper, serviceData, null);
            var result = await iotSendAsync.InvokeAsync();
            if (result.IsNullOrEmpty())
            {
                result = "false";
            }
            return Encoding.Default.GetBytes(result);
        }

        #region Event Mvvmbindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var detailDeviceSettings = IoTDeviceSettings.GetIoTDetailDeviceSettings(_signName, DeviceNumber);
                                               var editDialog = new EditIpAddressDialog("不填", 1, "不填", 0, DeviceHeader, IoTSocketType.Server);
                                               if (editDialog.ShowDialog() == true)
                                               {
                                                   detailDeviceSettings.DeviceHeader = editDialog.DeviceHeader;
                                                   detailDeviceSettings.DeviceNumber = editDialog.DeviceNumber;
                                                   if (IoTDeviceService.SaveSettings())
                                                   {
                                                       DeviceHeader = editDialog.DeviceHeader;
                                                       DeviceNumber = editDialog.DeviceNumber;
                                                   }
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

        private void InitTestUploadCheckedCommand()
        {
            TestUploadCheckedCommand = new RelayCommand(() =>
                                                        {
                                                            //_callback = new TLAutoDevicePushCallbackTest();
                                                            //_callback.Notify += (s, w) =>
                                                            //{
                                                            //    var text = Encoding.Default.GetString(w.Data);
                                                            //    WriteMsgWithLog(text, StatusNotificationType.NInfo);
                                                            //};
                                                            //var wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, IoTDeviceService.GetIoTDeviceService().WcfServiceAddress);
                                                            //wcfService.Send(proxy =>
                                                            //{
                                                            //    proxy.RegistControlDeviceEx("Test", new ControlInfo
                                                            //    {
                                                            //        ServiceKey = IoTDeviceService.Key,
                                                            //        Data = new IoTControlServiceData()
                                                            //        {
                                                            //            DeviceNumber = DeviceNumber,
                                                            //            Key = "Test",
                                                            //            ResultLength = 4,
                                                            //            SignName = _signName,
                                                            //            HasResult = true
                                                            //        }.ToBytes()
                                                            //    });
                                                            //});
                                                            IsShowServerReceivedInfo = true;
                                                        });
        }

        public RelayCommand TestUploadCheckedCommand { private set; get; }

        private void InitTestUploadUncheckedComma()
        {
            TestUploadUncheckedCommand = new RelayCommand(() =>
                                                          {
                                                              //var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, IoTDeviceService.GetIoTDeviceService().WcfServiceAddress);
                                                              //service.Send(proxy =>
                                                              //{
                                                              //    proxy.UnRegistControlDeviceEx("Test", IoTDeviceService.Key);
                                                              //});
                                                              IsShowServerReceivedInfo = false;
                                                          });
        }

        public RelayCommand TestUploadUncheckedCommand { private set; get; }

        private void InitTestClientConnectedCommand()
        {
            TestClientConnectedCommand = new RelayCommand(async () =>
                                                          {
                                                              if (Parent.IoTSocketType == IoTSocketType.Client)
                                                              {
                                                                  var socketSettings = IoTDeviceSettings.GetScoketSettingsFromDetailDevice(_signName, DeviceNumber);
                                                                  var serviceData = new IoTControlServiceData
                                                                                    {
                                                                                        SignName = socketSettings.SignName,
                                                                                        DeviceNumber = DeviceNumber,
                                                                                        Key = "Test",
                                                                                        Value = "value",
                                                                                        HasResult = true,
                                                                                        ResultLength = 4
                                                                                    };
                                                                  var resultInfo = await SendWcfCommandHelper.Send(serviceData);
                                                                  if (resultInfo != null)
                                                                  {
                                                                      var msgInfo = resultInfo.IsError ? resultInfo.ErrorMsg : (resultInfo.Data[0].ToBoolean() ? "测试成功。" : "测试失败。");
                                                                      var statusType = resultInfo.IsError ? StatusNotificationType.Error : (Encoding.Default.GetString(resultInfo.Data).ToBoolean() ? StatusNotificationType.RInfo : StatusNotificationType.NInfo);
                                                                      WriteMsgWithLog(msgInfo, statusType);
                                                                  }
                                                                  else
                                                                  {
                                                                      WriteMsgWithLog($"{SendWcfCommandHelper.ErrorInfoForCommandTimeOutOrException}", StatusNotificationType.Error);
                                                                  }
                                                              }
                                                          });
        }

        public RelayCommand TestClientConnectedCommand { private set; get; }

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