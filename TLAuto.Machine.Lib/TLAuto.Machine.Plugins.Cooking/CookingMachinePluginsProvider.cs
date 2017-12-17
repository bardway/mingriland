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

using TLAuto.Machine.Plugins.Cooking.Models;
using TLAuto.Machine.Plugins.Cooking.Models.Enums;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
#endregion

namespace TLAuto.Machine.Plugins.Cooking
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CookingMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        internal const string SignKey = "Cooking";
        private readonly List<CookingData> _cookingDatas = new List<CookingData>();
        private List<MachineButtonItem> _foodButtonItems;
        private volatile bool _isBreakSecond;
        private volatile bool _isBreakThird;
        private MachineButtonItem _speedButtonItem;
        private MachineButtonItem _startButtonItem;

        private void InitGameData()
        {
            var gameData = ConfigurationManager.AppSettings[$"{SignKey}Data"];
            var datas = gameData.Split('|');
            foreach (var data in datas)
            {
                var data2Nums = data.Split('-');
                var cookingData = new CookingData(data2Nums[0].Split(','), data2Nums[1].Split(':'), data2Nums[2], _speedButtonItem, _foodButtonItems);
                _cookingDatas.Add(cookingData);
            }
        }

        private void InitAppParam()
        {
            _foodButtonItems = ButtonItems.Take(12).ToList();
            _startButtonItem = ButtonItems[12];
            _speedButtonItem = ButtonItems[13];

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
            InitAppParam();
            InitGameData();
            ArduinoHelper.Connect();
            for (var i = 0; i < 12; i++)
            {
                ArduinoHelper.ShowLed2(0, i);
            }
            ArduinoHelper.ShowLed3(0, Led3Type.Temperature);
            ArduinoHelper.ShowLed3(0, Led3Type.Time);
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            while (true)
            {
                var gameIndex = 0;

                foreach (var cookingData in _cookingDatas)
                {
                    if ((gameIndex == 1) && _isBreakSecond)
                    {
                        gameIndex++;
                        continue;
                    }
                    if ((gameIndex == 2) && _isBreakThird)
                    {
                        continue;
                    }
                    Repeat:
                    var result = await SendWcfCommandPluginsHelper.InvokerQueryDiaitalSwitchWithAutoUpload(_startButtonItem, 60000 * 9);
                    if (!result)
                    {
                        goto Repeat;
                    }
                    await PlayStartBack(gameIndex);
                    if (cookingData.Excute(this))
                    {
                        var musicIndex = gameIndex;
                        if ((gameIndex == 0) && _isBreakSecond)
                        {
                            musicIndex++;
                        }
                        if ((gameIndex == 1) && _isBreakThird)
                        {
                            musicIndex++;
                        }
                        if (gameIndex == 0)
                        {
                            if (!(_isBreakSecond && _isBreakThird))
                            {
                                await PlayMusic0(SignKey, "第一轮结束.wav");
                            }
                        }
                        else
                        {
                            if (gameIndex == 1)
                            {
                                if (!_isBreakThird)
                                {
                                    await PlayMusic0(SignKey, "第二轮结束.wav");
                                }
                            }
                        }

                        await Task.Delay(2000);
                        gameIndex++;
                    }
                    else
                    {
                        await PlayTextMusicFromFirstItem("游戏超时，请重新开始");
                        goto Repeat;
                    }
                }
                break;
            }
            await PlayMusic0(SignKey, "line4.wav");
            GameCompleted();
        }

        private async Task PlayStartBack(int index)
        {
            switch (index)
            {
                case 0:
                    await PlayMusic0(SignKey, "做面开始语音.wav");
                    await Task.Delay(15000);
                    //await PlayMusic0(SignKey, "line5.wav");
                    //await Task.Delay(2100);
                    await PlayMusic0(SignKey, "2b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "1n.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "3b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "8n.wav");
                    await Task.Delay(1000);
                    break;
                case 1:
                    await PlayMusic0(SignKey, "line6.wav");
                    await Task.Delay(2100);
                    await PlayMusic0(SignKey, "1b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "3n.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "1b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "5n.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "1b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "11n.wav");
                    await Task.Delay(1000);
                    break;
                case 2:
                    await PlayMusic0(SignKey, "line7.wav");
                    await Task.Delay(2100);
                    await PlayMusic0(SignKey, "2b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "7n.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "2b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "9n.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "2b.wav");
                    await Task.Delay(1000);
                    await PlayMusic0(SignKey, "12n.wav");
                    await Task.Delay(1000);
                    break;
            }
        }

        private void GameCompleted()
        {
            ArduinoHelper.Close();
            OnGameOver();
        }
    }
}