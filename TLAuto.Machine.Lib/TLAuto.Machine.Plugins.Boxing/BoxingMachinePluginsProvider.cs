// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Machine.Plugins.Boxing.Models;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
#endregion

namespace TLAuto.Machine.Plugins.Boxing
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BoxingMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        internal const string SignKey = "Boxing";
        private readonly List<BoxingData> _boxingDatas = new List<BoxingData>();
        private List<MachineButtonItem> _checkButtonItems;
        private HitCheckTask _hitCheckTask;
        private volatile bool _isBreakSecond;
        private volatile bool _isBreakThird;
        private volatile bool _isFirst;
        private int _randomHitIndex;
        private MachineButtonItem _startButtonItem;

        private void InitGameData()
        {
            var gameData = ConfigurationManager.AppSettings[$"{SignKey}Data"];
            var datas = gameData.Split('|');
            foreach (var data in datas)
            {
                var boxingData = new BoxingData();
                var data2Nums = data.Split('-');
                boxingData.HitCount = data2Nums[0].ToInt32();
                boxingData.Delay = data2Nums[1].ToInt32();
                boxingData.Number = data2Nums[2].Split(',').Select(s => s.ToInt32()).ToList();
                _boxingDatas.Add(boxingData);
            }
            _startButtonItem = ButtonItems[ButtonItems.Count - 1];
            _checkButtonItems = ButtonItems.Take(8).ToList();

            _isBreakThird = true;
            MainUI.BreakSecond += MainUI_BreakSecond;
            MainUI.UnbreakSecond += MainUI_UnbreakSecond;
            MainUI.BreakThird += MainUI_BreakThird;
            MainUI.UnbreakThird += MainUI_UnbreakThird;
        }

        private void MainUI_UnbreakThird(object sender, EventArgs e)
        {
            _isBreakThird = false;
        }

        private void MainUI_BreakThird(object sender, EventArgs e)
        {
            _isBreakThird = true;
        }

        private void MainUI_UnbreakSecond(object sender, EventArgs e)
        {
            _isBreakSecond = false;
        }

        private void MainUI_BreakSecond(object sender, EventArgs e)
        {
            _isBreakSecond = true;
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            InitGameData();
            var buttonItem = ButtonItems[0];
            _hitCheckTask = new HitCheckTask(SignKey, buttonItem.ServiceAddress, buttonItem.DeviceNumber, buttonItem.SignName);
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            try
            {
                while (true)
                {
                    var endIndex = 1;
                    var isHitSecond = false;
                    var isHitThird = false;
                    foreach (var boxingData in _boxingDatas) //每一轮
                    {
                        if (!isHitSecond && _isBreakSecond && (endIndex > 1))
                        {
                            isHitSecond = true;
                            endIndex++;
                            continue;
                        }
                        if (!isHitThird && _isBreakThird && (endIndex > 2))
                        {
                            isHitThird = true;
                            continue;
                        }
                        Start:
                        if (_isFirst)
                        {
                            var result2 = await CheckStartButton();
                            if (!result2)
                            {
                                await Task.Delay(1000);
                                goto Start;
                            }
                        }
                        _isFirst = true;
                        for (var i = 0; i < boxingData.HitCount; i++)
                        {
                            var buttonIndex = boxingData.Number[i];
                            var imageIndex = boxingData.GetImageIndex(buttonIndex - 1); //打击图案编号
                            var delay = boxingData.Delay * 1000; //检测打击时间
                            await boxingData.PlayMusic(imageIndex, this, endIndex - 1);
                            var needButtonItem = ButtonItems[imageIndex - 1];
                            OnNotification(new NotificationEventArgs("检测是否击打，对应开关板子编号为：" + needButtonItem.Number));
                            var hitCheckTaskAsync = new HitCheckTaskAsync(_hitCheckTask, _checkButtonItems, ButtonItems[imageIndex - 1], delay);
                            var result = await hitCheckTaskAsync.InvokeAsync();
                            if (!result)
                            {
                                OnNotification(new NotificationEventArgs("没检测到击打或超时或击打错误。"));
                                await Failed();
                                goto Start;
                            }
                            OnNotification(new NotificationEventArgs("检测到被击中."));
                            PlayHitMusic();
                        }
                        var musicIndex = endIndex;
                        if (_isBreakSecond)
                        {
                            musicIndex++;
                        }
                        if (_isBreakThird)
                        {
                            musicIndex++;
                        }
                        await RestartMusic(musicIndex);
                        endIndex++;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                OnNotification(new NotificationEventArgs(ex.Message));
            }
            GameCompleted();
        }

        private async Task Failed()
        {
            await PlayMusic0(SignKey, "Error.wav", "Error");
            await Task.Delay(2000);
        }

        private async Task<bool> CheckStartButton()
        {
            try
            {
                var resultItem = await SendWcfCommandPluginsHelper.InvokerQueryDiaitalSwitchWithAutoUpload(_startButtonItem, 60000);
                if (resultItem)
                {
                    await PlayMusic0(SignKey, "Go.wav", "Go");
                    await Task.Delay(2000);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void GameCompleted()
        {
            await PlayMusic0(SignKey, "GameOver.wav", "GameOver");
            await Task.Delay(2000);
            OnGameOver();
        }

        private async Task RestartMusic(int endIndex)
        {
            if (endIndex == 1)
            {
                await PlayMusic0(SignKey, "End1.wav", "End");
                await Task.Delay(9000);
            }
            else
            {
                if (endIndex == 2)
                {
                    await PlayMusic0(SignKey, "End2.wav", "End");
                    await Task.Delay(4000);
                }
            }
        }

        private async void PlayHitMusic()
        {
            switch (_randomHitIndex)
            {
                case 0:
                    await PlayMusic0(SignKey, "hit1.wav", "hit");
                    await Task.Delay(1000);
                    _randomHitIndex++;
                    break;
                case 1:
                    await PlayMusic0(SignKey, "hit2.wav", "hit");
                    await Task.Delay(1000);
                    _randomHitIndex++;
                    break;
                case 2:
                    await PlayMusic0(SignKey, "hit3.wav", "hit");
                    await Task.Delay(1000);
                    _randomHitIndex = 0;
                    break;
                default:
                    _randomHitIndex = 0;
                    break;
            }
        }
    }
}