// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TLAuto.Base.Extensions;
using TLAuto.Notification.Contracts;
using TLAuto.Notification.ServerHost.Common;
using TLAuto.Notification.ServerHost.Dialog;
using TLAuto.Notification.ServerHost.IocService;
using TLAuto.Wcf.Client;
using TLAuto.Wcf.Server;
using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Notification.ServerHost.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly NetTcpWcfServerService _tlautoDeviceMusicService = new NetTcpWcfServerService(TimeSpan.FromMinutes(10), true);

        #region Ctors
        public MainViewModel(INotificationSourceService notificationSourceService)
        {
            notificationSourceService.SetNotificationUI(NotificationVm);
            _tlautoDeviceMusicService.Opened += MusicSourceService_Opened;
            _tlautoDeviceMusicService.Closed += NotificationSourceService_Closed;
            _tlautoDeviceMusicService.Error += NotificationSourceService_Error;
            InitEventMvvmBindings();
        }
        #endregion

        #region Methods
        private void InitEventMvvmBindings()
        {
            InitLoadedCommand();
            InitClosingCommand();
            InitStartServiceHostCommand();
            InitStopServiceHostCommand();
            InitSettingsServiceAddressCommand();
            InitAddAppKeyCommand();
            InitGetAppStatusCommand();
            InitShutDownCommand();
            InitStopAppCommand();
        }

        private void WriteMsg(string msg)
        {
            Messenger.Default.Send(new NotificationMessage(msg));
        }

        private async void MusicSourceService_Opened(object sender, EventArgs e)
        {
            IsOpen = true;
            WriteMsg("通信服务已经启动。");
            var result = await ViewModelLocator.Instance.NotificationSourceService.TestConnected();
            if (result)
            {
                WriteMsg("尝试调用接口成功。");
            }
        }

        private void NotificationSourceService_Closed(object sender, WcfServerServiceMessageEventArgs e)
        {
            IsOpen = false;
            WriteMsg("通信服务已经关闭。");
        }

        private void NotificationSourceService_Error(object sender, WcfServerServiceErrorMessageEventArgs e)
        {
            WriteMsg("开启通讯服务出现错误，原因：" + e.Msg);
        }

        private void StartMusicService()
        {
            if (IsOpen)
            {
                return;
            }
            if (ConfigHelper.ServiceAddress.IsNullOrEmpty())
            {
                MessageBox.Show("服务地址参数为空。");
                return;
            }
            _tlautoDeviceMusicService.StartWcfService(ConfigHelper.ServiceAddress, typeof(TLNotificationService), typeof(ITLNotification));
        }

        private void StopMusicService()
        {
            if (IsOpen)
            {
                _tlautoDeviceMusicService.StopWcfService();
            }
        }

        private async Task<T> SendCommand<T>(Func<ITLNotification, Task<T>> func)
        {
            if (!IsOpen)
            {
                MessageBox.Show("通讯服务未启动。");
                return default(T);
            }
            NotificationVm.IsSendCommand = true;
            var notificationSendCommand = new NetTcpWcfClientService<ITLNotification>(ConfigHelper.ServiceAddress);
            notificationSendCommand.Error += (s, e) => { WriteMsg(e.Msg); };
            notificationSendCommand.CommunicationError += (s, e) => { WriteMsg(e.Msg); };
            notificationSendCommand.TimeoutError += (s, e) => { WriteMsg(e.Msg); };
            var result = await notificationSendCommand.SendAsync(func);
            NotificationVm.IsSendCommand = false;
            return result;
        }
        #endregion

        #region Properties
        private bool _isOpen;

        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                RaisePropertyChanged();
            }
            get => _isOpen;
        }

        public NotificationViewModel NotificationVm { get; } = new NotificationViewModel();
        #endregion

        #region Event MvvmBindings
        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(StartMusicService);
        }

        public RelayCommand LoadedCommand { private set; get; }

        private void InitClosingCommand()
        {
            ClosingCommand = new RelayCommand<CancelEventArgs>(e =>
                                                               {
                                                                   if (IsOpen)
                                                                   {
                                                                       if (MessageBox.Show("服务正在运行中，还是需要关闭吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                                                       {
                                                                           e.Cancel = true;
                                                                       }
                                                                       StopMusicService();
                                                                   }
                                                               });
        }

        public RelayCommand<CancelEventArgs> ClosingCommand { private set; get; }

        #region Menu Settings
        private void InitStartServiceHostCommand()
        {
            StartServiceHostCommand = new RelayCommand(StartMusicService);
        }

        public RelayCommand StartServiceHostCommand { private set; get; }

        private void InitStopServiceHostCommand()
        {
            StopServiceHostCommand = new RelayCommand(StopMusicService);
        }

        public RelayCommand StopServiceHostCommand { private set; get; }

        private void InitSettingsServiceAddressCommand()
        {
            SettingsServiceAddressCommand = new RelayCommand(() =>
                                                             {
                                                                 var sad = new SettingServiceAddressDialog(ConfigHelper.ServiceAddress);
                                                                 if (sad.ShowDialog() == true)
                                                                 {
                                                                     ConfigHelper.SaveConfig(sad.ServiceAddress);
                                                                 }
                                                             });
        }

        public RelayCommand SettingsServiceAddressCommand { private set; get; }
        #endregion

        #region Control
        private void InitAddAppKeyCommand()
        {
            AddAppKeyCommand = new RelayCommand(async () =>
                                                {
                                                    var end = new EditNotificationInfoDialog();
                                                    if (end.ShowDialog() == true)
                                                    {
                                                        var appKey = end.AppKey;
                                                        var appStatusType = end.SelectedAppStatusType;
                                                        var result = await SendCommand(async proxy => await proxy.AddAppStatus(appKey, appStatusType));
                                                        Messenger.Default.Send(new NotificationMessage(result ? "添加App状态命令调用成功" : "App状态命令调用失败"));
                                                    }
                                                });
        }

        public RelayCommand AddAppKeyCommand { private set; get; }

        private void InitGetAppStatusCommand()
        {
            GetAppStatusCommand = new RelayCommand(async () =>
                                                   {
                                                       var ead = new EditAppKeyDialog();
                                                       if (ead.ShowDialog() == true)
                                                       {
                                                           var appKey = ead.AppKey;
                                                           var result = await SendCommand(async proxy => await proxy.GetAppStatus(appKey));
                                                           Messenger.Default.Send(new NotificationMessage("获取App状态命令调用成功"));
                                                           MessageBox.Show(string.Format("标识符：{0} 状态为：{1}", appKey, result));
                                                       }
                                                   });
        }

        public RelayCommand GetAppStatusCommand { private set; get; }

        private void InitShutDownCommand()
        {
            ShutDownCommand = new RelayCommand(async () =>
                                               {
                                                   var dr = MessageBox.Show(@"确定要关机吗？请保存信息后按'是'，不关机按'否'", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Information);
                                                   if (dr == MessageBoxResult.Yes)
                                                   {
                                                       await SendCommand(async proxy => await proxy.ShutDown());
                                                   }
                                               });
        }

        public RelayCommand ShutDownCommand { private set; get; }

        private void InitStopAppCommand()
        {
            StopAppCommand = new RelayCommand(async () =>
                                              {
                                                  var dialog = new EditAppNameDialog();
                                                  if (dialog.ShowDialog() == true)
                                                  {
                                                      var processName = dialog.ProcessName;
                                                      await SendCommand(async proxy => await proxy.StopApp(processName));
                                                  }
                                              });
        }

        public RelayCommand StopAppCommand { private set; get; }
        #endregion
        #endregion
    }
}