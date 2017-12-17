// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Candle.Models;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.Candle
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CandleProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Candle";
        private readonly ConcurrentDictionary<int, CheckButtonItem> _dicButtonItems = new ConcurrentDictionary<int, CheckButtonItem>();
        private TLAutoDevicePushCallback _callback;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        private void Init()
        {
            foreach (var machineButtonItem in ButtonItems)
            {
                _dicButtonItems.TryAdd(machineButtonItem.Number, new CheckButtonItem(machineButtonItem));
            }
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(GameLogic);
        }

        private void GameLogic()
        {
            RegButtonCheckNotification();
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

        private async void Callback_Notify(object sender, WcfResultInfo e)
        {
            if (!e.IsError)
            {
                var switchItem = e.Data.ToObject<SwitchItem>();
                if ((switchItem.SwitchNumber == 5) || (switchItem.SwitchNumber == 4))
                {
                    OnNotification(new NotificationEventArgs("开关编号:" + switchItem.SwitchNumber +
                                                             " 状态：" + (switchItem.SwitchStatus == SwitchStatus.NC ? "吸合" : "非吸合")));
                    if (_dicButtonItems.ContainsKey(switchItem.SwitchNumber))
                    {
                        CheckButtonItem buttonItem;
                        if (_dicButtonItems.TryGetValue(switchItem.SwitchNumber, out buttonItem))
                        {
                            buttonItem.IsPress = switchItem.SwitchStatus == SwitchStatus.NC;
                            if (_dicButtonItems.Values.All(s => s.IsPress))
                            {
                                PlayMusic0(SignKey, "line1.wav");
                            }
                        }

                        //if (!_dicButtonItems.TryAdd(switchItem.SwitchNumber, buttonItem))
                        //{
                        //    OnNotification(new NotificationEventArgs("开关编号:" + switchItem.SwitchNumber + "添加到集合失败，如果当前只需检测一个开关是否吸合，可以直接跳过。"));
                        //}
                        //else
                        //{
                        //    //var str = _dicButtonItems.Values.Aggregate(string.Empty, (current, checkButtonItem) => current + $"{checkButtonItem.ButtonItem.Number} {checkButtonItem.IsPressStatus} ");
                        //    //OnNotification(new NotificationEventArgs(str));
                        //}
                    }
                }
            }
        }
    }
}