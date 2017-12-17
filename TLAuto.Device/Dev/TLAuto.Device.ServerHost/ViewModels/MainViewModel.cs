// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Device.Contracts;
using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.Extension.Core;
using TLAuto.Device.ServerHost.Config;
using TLAuto.Device.ServerHost.Models;
using TLAuto.Device.ServerHost.Views;
using TLAuto.Device.ServerHost.WcfService;
using TLAuto.Log;
using TLAuto.Wcf.Server;
using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Device.ServerHost.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly LogWraper _log = new LogWraper("ReceiveWcfService");
        private readonly NetTcpWcfServerService _tlautoDeviceIocService = new NetTcpWcfServerService();

        private bool _isEnabledOpenServiceButton = true;

        private bool _isStartWcfService;

        private string _wcfServiceErrorInfo;

        public MainViewModel()
        {
            _tlautoDeviceIocService.Error += TLAutoDeviceIocService_Error;
            ;
            InitEventCommands();
        }

        public bool IsEnabledOpenServiceButton
        {
            set
            {
                _isEnabledOpenServiceButton = value;
                RaisePropertyChanged();
            }
            get => _isEnabledOpenServiceButton;
        }

        public bool IsStartWcfService
        {
            set
            {
                _isStartWcfService = value;
                RaisePropertyChanged();
            }
            get => _isStartWcfService;
        }

        public string WcfServiceErrorInfo
        {
            set
            {
                _wcfServiceErrorInfo = value;
                RaisePropertyChanged();
            }
            get => _wcfServiceErrorInfo;
        }

        public ObservableCollection<DeviceServiceInfo> DeviceServiceInfos { get; } = new ObservableCollection<DeviceServiceInfo>();

        #region Methods
        private void InitEventCommands()
        {
            InitLoadedCommand();
            InitClosingCommand();
            InitAddDeviceCommand();
            InitRemoveDeviceCommand();
            InitEditDeviceServiceCommand();
            InitOpenWcfServiceCommand();
            InitOpenExplorerCommand();
        }

        private void TLAutoDeviceIocService_Error(object sender, WcfServerServiceErrorMessageEventArgs e)
        {
            _log.Error(e.Msg, e.Ex);
            WcfServiceErrorInfo = DateTime.Now + " " + e.Msg;
        }

        private void RemoveDeviceService(string serverTitle)
        {
            var deviceServiceInfo = DeviceServiceInfos.FirstOrDefault(s => s.Description == serverTitle);
            if (deviceServiceInfo != null)
            {
                if (MessageBox.Show($"确认删除 {serverTitle} 通信服务吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    deviceServiceInfo.Cleanup();
                    DeviceServiceInfos.Remove(deviceServiceInfo);
                }
            }
        }

        private async Task OpenWcfService()
        {
            IsEnabledOpenServiceButton = false;
            if (!IsStartWcfService)
            {
                IsStartWcfService = await _tlautoDeviceIocService.StartWcfService(ConfigHelper.WcfServiceAddress,
                                                                                  typeof(TLAutoDeviceService),
                                                                                  typeof(ITLAutoDevice),
                                                                                  typeof(ITLAutoDevicePushService));
            }
            IsEnabledOpenServiceButton = true;
        }

        private void CloseWcfService()
        {
            IsEnabledOpenServiceButton = false;
            _tlautoDeviceIocService.StopWcfService();
            IsStartWcfService = false;
            IsEnabledOpenServiceButton = true;
        }

        private bool SaveDeviceSettings(IDeviceService deviceService)
        {
            Exception ex;
            var result = deviceService.SaveDeviceSettings(out ex);
            if (!result)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        #endregion

        #region Event MvvmBindings
        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(async () =>
                                             {
                                                 if (!TLDeviceExtensionsService.Instance.LoadDeviceExtensions())
                                                 {
                                                     MessageBox.Show("通信设备服务加载失败。");
                                                     return;
                                                 }
                                                 foreach (var deviceService in TLDeviceExtensionsService.Instance.DeviceServices)
                                                 {
                                                     if (deviceService.DeviceSettings.Exists)
                                                     {
                                                         deviceService.Init(ConfigHelper.WcfServiceAddress);
                                                         DeviceServiceInfos.Add(new DeviceServiceInfo(deviceService));
                                                     }
                                                 }
                                                 if (ConfigHelper.IsAutoStartWcf)
                                                 {
                                                     await OpenWcfService();
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }

        private void InitClosingCommand()
        {
            ClosingCommand = new RelayCommand<CancelEventArgs>(e =>
                                                               {
                                                                   if (IsStartWcfService)
                                                                   {
                                                                       if (MessageBox.Show("确认关闭设备通信服务吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                                                       {
                                                                           CloseWcfService();
                                                                       }
                                                                       else
                                                                       {
                                                                           e.Cancel = true;
                                                                       }
                                                                   }
                                                               });
        }

        public RelayCommand<CancelEventArgs> ClosingCommand { private set; get; }

        private void InitAddDeviceCommand()
        {
            AddDeviceCommand = new RelayCommand(() =>
                                                {
                                                    var dialog = new AddDeviceServiceDialogView();
                                                    if (dialog.ShowDialog() == true)
                                                    {
                                                        var serviceKey = dialog.ServiceKey;
                                                        var deviceService = TLDeviceExtensionsService.Instance.GetDeviceService(serviceKey);
                                                        deviceService.Init(ConfigHelper.WcfServiceAddress);
                                                        if (SaveDeviceSettings(deviceService))
                                                        {
                                                            DeviceServiceInfos.Add(new DeviceServiceInfo(deviceService));
                                                        }
                                                    }
                                                });
        }

        public RelayCommand AddDeviceCommand { private set; get; }

        private void InitRemoveDeviceCommand()
        {
            RemoveDeviceCommand = new RelayCommand(() =>
                                                   {
                                                       var dialog = new RemoveItemsView("删除设备通信服务", DeviceServiceInfos.Select(s => s.Description).ToList());
                                                       if (dialog.ShowDialog() == true)
                                                       {
                                                           var deleteItems = dialog.SelectedItems;
                                                           foreach (var item in deleteItems)
                                                           {
                                                               RemoveDeviceService(item);
                                                           }
                                                       }
                                                   });
        }

        public RelayCommand RemoveDeviceCommand { private set; get; }

        private void InitEditDeviceServiceCommand()
        {
            EditWcfDeviceServiceCommand = new RelayCommand(() =>
                                                           {
                                                               var dialog = new EditServiceInfoView(ConfigHelper.WcfServiceAddress, ConfigHelper.IsAutoStartWcf);
                                                               if (dialog.ShowDialog() == true)
                                                               {
                                                                   ConfigHelper.SaveConfig(dialog.ServiceAddress, dialog.IsAutoStart);
                                                                   MessageBox.Show("更改成功，但需要重新应用程序才能生效。（手动重启）", "提示", MessageBoxButton.OK);
                                                               }
                                                           });
        }

        public RelayCommand EditWcfDeviceServiceCommand { private set; get; }

        private void InitOpenExplorerCommand()
        {
            OpenExplorerCommand = new RelayCommand(() => { Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory); });
        }

        public RelayCommand OpenExplorerCommand { private set; get; }

        private void InitOpenWcfServiceCommand()
        {
            OpenWcfServiceCommand = new RelayCommand(async () =>
                                                     {
                                                         if (!IsStartWcfService)
                                                         {
                                                             await OpenWcfService();
                                                         }
                                                         else
                                                         {
                                                             if (MessageBox.Show("确认关闭通信服务吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                                             {
                                                                 CloseWcfService();
                                                             }
                                                         }
                                                     });
        }

        public RelayCommand OpenWcfServiceCommand { private set; get; }
        #endregion
    }
}