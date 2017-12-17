// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Machine.Plugins.Sudoku.Models;
using TLAuto.Wcf.Client;

using Timer = System.Timers.Timer;
#endregion

namespace TLAuto.Machine.Plugins.Sudoku
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SudokuMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Sudoku";
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        private readonly List<SudokuData> _sudokuDatas = new List<SudokuData>();
        private readonly Timer _timer = new Timer(1000) {AutoReset = false};
        private volatile bool _isCompleted;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        private void InitGameData()
        {
            var gameData = ConfigurationManager.AppSettings[$"{SignKey}Data"];
            var datas = gameData.Split('|');
            foreach (var data in datas)
            {
                var sudokuData = new SudokuData();
                var data2Nums = data.Split('-');
                sudokuData.ButtonIndex = data2Nums[0].ToInt32();
                var relayNums = data2Nums[1].Split(',');
                foreach (var relayNum in relayNums)
                {
                    sudokuData.RelayIndexs.Add(relayNum.ToInt32());
                }
                _sudokuDatas.Add(sudokuData);
            }
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            InitGameData();
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            for (var i = 0; i < RelayItems.Count; i++)
            {
                if (i > 0)
                {
                    await RelayItems[i].Control(true);
                }
            }
            while (true)
            {
                try
                {
                    var callback = new TLAutoDevicePushCallback();
                    callback.Notify += Callback_Notify;
                    RegButtonCheckNotification(ButtonItems, callback);
                    _manualResetEvent.Reset();
                    if (_manualResetEvent.WaitOne(60000))
                    {
                        break;
                    }
                    UnregButtonCheckNotification(ButtonItems[0].DeviceNumber, ButtonItems[1].ServiceAddress, callback);
                }
                catch (Exception ex)
                {
                    OnNotification(new NotificationEventArgs(ex.Message));
                }
                await Task.Delay(1000);
            }
            GameCompleted();
        }

        private async void Callback_Notify(object sender, WcfResultInfo e)
        {
            if (e.IsError)
            {
                return;
            }
            var switchItem = e.Data.ToObject<SwitchItem>();
            var buttonItem = ButtonItems.FirstOrDefault(s => s.Number == switchItem.SwitchNumber);
            if (buttonItem != null)
            {
                if (!_timer.Enabled)
                {
                    _timer.Start();
                }
                else
                {
                    return;
                }
                if (_isCompleted)
                {
                    return;
                }
                var findIndex = ButtonItems.IndexOf(buttonItem) + 1;
                var sudokuData = _sudokuDatas.Find(s => s.ButtonIndex == findIndex);
                foreach (var relayIndex in sudokuData.RelayIndexs)
                {
                    await RelayItems[relayIndex - 1].Control();
                }
                await PlayMusic0(SignKey, "ding.wav");
                //await Task.Delay(1000);
                var result = RelayItems.All(s => s.IsNo);
                if (result)
                {
                    _isCompleted = true;
                    _manualResetEvent.Set();
                }
            }
        }

        private void GameCompleted()
        {
            OnGameOver();
        }

        private bool RegButtonCheckNotification(List<MachineButtonItem> checkButtonItems, TLAutoDevicePushCallback callback)
        {
            _wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, checkButtonItems[0].ServiceAddress);
            var result = _wcfService.Send(proxy =>
                                          {
                                              proxy.RegistControlDeviceEx(SignKey + checkButtonItems[0].DeviceNumber,
                                                                          new ControlInfo
                                                                          {
                                                                              ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                              Data = new PLCControlServiceData
                                                                                     {
                                                                                         ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                         DeviceNumber = checkButtonItems[0].DeviceNumber,
                                                                                         PortSignName = checkButtonItems[0].SignName
                                                                                     }.ToBytes()
                                                                          });
                                          });
            return result;
        }

        private void UnregButtonCheckNotification(int deviceNumber, string serviceAddress, TLAutoDevicePushCallback callback)
        {
            try
            {
                _wcfService.Close();
                var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, serviceAddress);
                service.Send(proxy => { proxy.UnRegistControlDeviceEx(SignKey + deviceNumber, CommonConfigHelper.PLCServiceKey); });
                service.Close();
            }
            catch (Exception ex) { }
        }
    }
}