// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.CheckPressButtons.Models;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.CheckPressButtons
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CheckPressButtonsMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "CheckPressButtons";
        private readonly ConcurrentDictionary<int, CheckButtonItem> _dicButtonItems = new ConcurrentDictionary<int, CheckButtonItem>();
        private readonly object _lock = new object();
        private readonly Timer _timer = new Timer(1000) {AutoReset = false};
        private TLAutoDevicePushCallback _callback;
        private volatile bool _isGameEnd;
        private int _timeDelay;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        private void InitPressButtonItems()
        {
            _timer.Elapsed += Timer_Elapsed;
            foreach (var machineButtonItem in ButtonItems)
            {
                _dicButtonItems.TryAdd(machineButtonItem.Number, new CheckButtonItem(machineButtonItem));
            }
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            OnNotification(new NotificationEventArgs(ButtonItems.Select(s => s.Number).ToList().ToText()));
            InitPressButtonItems();
            Task.Factory.StartNew(GameLogic);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //else
            //{
            //    foreach (var key in _dicButtonItems.Keys)
            //    {
            //        CheckButtonItem buttonItem;
            //        _dicButtonItems.TryRemove(key, out buttonItem);
            //        buttonItem.IsPress = false;
            //        _dicButtonItems.TryAdd(key, buttonItem);
            //    }
            //}
            _timeDelay += 1;
            if (_timeDelay == 60 * 9)
            {
                UnregButtonCheckNotification();
                _timeDelay = 0;
                GameLogic();
                return;
            }
            _timer.Start();
        }

        private void GameCompleted()
        {
            lock (_lock)
            {
                if (_isGameEnd)
                {
                    return;
                }
                _isGameEnd = true;
                //PlayTextMusicFromFirstItem("游戏完成");
                OnGameOver();
            }
        }

        private async void GameLogic()
        {
            if (RegButtonCheckNotification())
            {
                _timer.Start();
            }
            else
            {
                OnNotification(new NotificationEventArgs("注册开关检测事件失败，等待1S重新注册。"));
                UnregButtonCheckNotification();
                await Task.Delay(1000);
                GameLogic();
            }
        }

        private bool RegButtonCheckNotification()
        {
            try
            {
                if (_callback != null)
                {
                    _callback.Notify -= Callback_Notify;
                }
                _callback = new TLAutoDevicePushCallback();
                _callback.Notify += Callback_Notify;
                var buttonItem = ButtonItems[0];
                _wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, buttonItem.ServiceAddress);
                var result = _wcfService.Send(proxy =>
                                              {
                                                  proxy.RegistControlDeviceEx(SignKey,
                                                                              new ControlInfo
                                                                              {
                                                                                  ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                  Data = new PLCControlServiceData
                                                                                         {
                                                                                             ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                             DeviceNumber = buttonItem.DeviceNumber,
                                                                                             PortSignName = buttonItem.SignName
                                                                                         }.ToBytes()
                                                                              });
                                              });
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void UnregButtonCheckNotification()
        {
            try
            {
                _wcfService.Close();
                var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, ButtonItems[0].ServiceAddress);
                service.Send(proxy => { proxy.UnRegistControlDeviceEx(SignKey, CommonConfigHelper.PLCServiceKey); });
                service.Close();
            }
            catch (Exception ex) { }
        }

        private void Callback_Notify(object sender, WcfResultInfo e)
        {
            if (!e.IsError)
            {
                var switchItem = e.Data.ToObject<SwitchItem>();
                if (_dicButtonItems.ContainsKey(switchItem.SwitchNumber))
                {
                    CheckButtonItem buttonItem;
                    _dicButtonItems.TryRemove(switchItem.SwitchNumber, out buttonItem);
                    buttonItem.IsPress = switchItem.SwitchStatus == SwitchStatus.NC;
                    if (!_dicButtonItems.TryAdd(switchItem.SwitchNumber, buttonItem))
                    {
                        OnNotification(new NotificationEventArgs("开关编号:" + switchItem.SwitchNumber + "添加到集合失败，如果当前只需检测一个开关是否吸合，可以直接跳过。"));
                    }
                    else
                    {
                        var str = _dicButtonItems.Values.Aggregate(string.Empty, (current, checkButtonItem) => current + $"{checkButtonItem.ButtonItem.Number} {checkButtonItem.IsPressStatus} ");
                        OnNotification(new NotificationEventArgs(str));
                        if (_dicButtonItems.Values.All(s => s.IsPress))
                        {
                            UnregButtonCheckNotification();
                            GameCompleted();
                            //return;
                        }
                    }
                }
            }
        }
    }
}