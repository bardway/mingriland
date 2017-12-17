// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Machine.App.Common;
using TLAuto.Machine.App.IocService;
using TLAuto.Machine.Controls.Models;
using TLAuto.Machine.Controls.Models.Enums;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
#endregion

namespace TLAuto.Machine.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IAppParamSourceService _appParamSourceService;
        private readonly List<MusicItem> _musicItems = new List<MusicItem>();

        private object _contentView;

        private DifficulySystemType _difficulyType;
        private bool _isLoaded;

        private string _title = ConfigHelper.Title;

        public MainViewModel(IAppParamSourceService appParamSourceService)
        {
            _appParamSourceService = appParamSourceService;
            MachineBuilder.Instance.Init(ConfigHelper.MachineKey);
            MachineBuilder.Instance.MachinePluginsProvider.Notification += MachinePluginsProvider_Notification;
            MachineBuilder.Instance.MachinePluginsProvider.GameOver += MachinePluginsProvider_GameOver;
            InitLoadedCommand();
        }

        public object ContentView
        {
            set
            {
                _contentView = value;
                RaisePropertyChanged();
            }
            get => _contentView;
        }

        public DifficulySystemType DifficulyType
        {
            set
            {
                _difficulyType = value;
                RaisePropertyChanged();
            }
            get => _difficulyType;
        }

        public string Title
        {
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
            get => _title;
        }

        public ObservableCollection<SwitchItem> SwitchItems { get; } = new ObservableCollection<SwitchItem>();

        public ObservableCollection<RelayItem> RelayItems { get; } = new ObservableCollection<RelayItem>();

        private async void MachinePluginsProvider_GameOver(object sender, EventArgs e)
        {
            await SendWcfCommandHelper.InvokerNotificationForStop(ConfigHelper.MachineKey);
            MachineBuilder.Instance.MachinePluginsProvider.Dispose();
            Environment.Exit(-1);
        }

        private void MachinePluginsProvider_Notification(object sender, NotificationEventArgs e)
        {
            Debug.WriteLine(e.Notification);
            SendWcfCommandHelper.WriteMsg(e.Notification);
        }

        private void InitAppParamInfo()
        {
            var inputBoardParamInfos = _appParamSourceService.GetAppInputBoardParamInfos();
            var outputBoardParamInfos = _appParamSourceService.GetAppOutputBoardParamInfos();
            var musicParamInfos = _appParamSourceService.GetAppMusicParamInfos();
            var machineButtonItems = new List<MachineButtonItem>();
            var machineRelayItems = new List<MachineRelayItem>();
            var machineMusicItems = new List<MachineMusicItem>();
            foreach (var paramInfo in inputBoardParamInfos)
            {
                var switchItem = new SwitchItem(paramInfo.DeviceNumber, paramInfo.Number, paramInfo.ServiceAddress, paramInfo.PortName, SwitchItems.Count + 1);
                switchItem.Click += async (s, e) =>
                                    {
                                        var result = await SendWcfCommandHelper.InvokerSimulationDigitalSwitch(switchItem);
                                        if (!result)
                                        {
                                            MessageBox.Show("模拟开关点击失败!");
                                        }
                                    };
                SwitchItems.Add(switchItem);
                machineButtonItems.Add(new MachineButtonItem(paramInfo.DeviceNumber, paramInfo.Number, paramInfo.ServiceAddress, paramInfo.PortName));
            }
            foreach (var paramInfo in outputBoardParamInfos)
            {
                var relayItem = new RelayItem(paramInfo.DeviceNumber, paramInfo.Number, paramInfo.ServiceAddress, paramInfo.PortName, RelayItems.Count + 1);
                RelayItems.Add(relayItem);
                var machineRelayItem = new MachineRelayItem(paramInfo.DeviceNumber, paramInfo.Number, paramInfo.ServiceAddress, paramInfo.PortName);
                machineRelayItem.RelayStatusChanged += (s, e) => { relayItem.IsOpen = machineRelayItem.IsNo; };
                machineRelayItems.Add(machineRelayItem);
            }
            foreach (var paramInfo in musicParamInfos)
            {
                _musicItems.Add(new MusicItem(paramInfo));
                machineMusicItems.Add(new MachineMusicItem(paramInfo));
            }
            MachineBuilder.Instance.MachinePluginsProvider.InitDeviceParam(machineButtonItems, machineRelayItems, machineMusicItems);
        }

        #region Events Mvvmbindings
        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(async () =>
                                             {
                                                 if (_isLoaded)
                                                 {
                                                     return;
                                                 }
                                                 _isLoaded = true;
                                                 InitAppParamInfo();
                                                 DifficulyType = ConfigHelper.DiffType;
                                                 ContentView = MachineBuilder.Instance.MachinePluginsProvider.View;
                                                 var result = await SendWcfCommandHelper.InvokerNotificationForStart(ConfigHelper.MachineKey);
                                                 if (result)
                                                 {
                                                     SendWcfCommandHelper.WriteMsg("启动状态报告成功。");
                                                     MachineBuilder.Instance.MachinePluginsProvider.StartGame(ConfigHelper.DiffType == DifficulySystemType.Low ? DifficultyLevelType.Low : DifficultyLevelType.High, ConfigHelper.Args);
                                                 }
                                                 else
                                                 {
                                                     SendWcfCommandHelper.WriteMsg("启动状态报告失败。");
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}