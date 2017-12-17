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
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.HitMouse.Models;
using TLAuto.Machine.Plugins.HitMouse.Models.Attributes;
using TLAuto.Machine.Plugins.HitMouse.Models.Behaviors;
using TLAuto.Machine.Plugins.HitMouse.Models.Enums;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HitMouseMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        internal const string SignKey = "HitMouse";
        private readonly List<HitStyleData> _hitStyleDatas = new List<HitStyleData>();

        private readonly List<Tuple<int, int, string>> _hitTypes = new List<Tuple<int, int, string>>();
        private readonly int _queryTime = 60000 * 9;
        internal MachineButtonItem _beginButtonItem; //开始按钮
        internal List<MachineRelayItem> _chargeLightRelayItems = new List<MachineRelayItem>(); //10个充能灯
        private int _gameCount;
        private HitType _hitBuff = HitType.Default;
        private bool _isBreakThird;
        internal bool _isChanged;
        internal List<MachineButtonItem> _pointLightButtonItems; //20个指示灯按钮
        internal List<MachineRelayItem> _pointLightRelayItems; //20个指示灯
        internal List<MachineButtonItem> _questionLightButtonItems = new List<MachineButtonItem>(); //8个问号灯按钮
        internal List<MachineRelayItem> _questionLightRelayItems = new List<MachineRelayItem>(); //8个问号灯
        internal MachineRelayItem _roomLightRelayItem; //房间背景灯
        internal List<MachineRelayItem> _styleLightRelayItems = new List<MachineRelayItem>(); //8个招式灯

        private void InitGameData()
        {
            _hitTypes.Add(new Tuple<int, int, string>(1, 4, "buff.wav"));
            _hitTypes.Add(new Tuple<int, int, string>(2, 1, "buff.wav"));
            _hitTypes.Add(new Tuple<int, int, string>(3, 7, "debuff.wav"));

            var gameData = ConfigurationManager.AppSettings["HitStyleLightData"];
            var datas = gameData.Split('|');
            foreach (var data in datas)
            {
                var data2Nums = data.Split('-');
                var questionLightButtonIndex = data2Nums[0].ToInt32();
                var lightIndexs = data2Nums[1].Split(',').Select(s => s.ToInt32()).ToList();
                var hitMouseData = new HitStyleData(questionLightButtonIndex, lightIndexs);
                _hitStyleDatas.Add(hitMouseData);
            }

            _gameCount = ConfigurationManager.AppSettings["HitCount"].ToInt32();
        }

        private List<HitData> GetHitDatas()
        {
            var hitDatas = new List<HitData>();
            var gameData = ConfigurationManager.AppSettings[_hitBuff.GetEnumDescriptionAttribute()];
            for (var i = 0; i < 10; i++)
            {
                var dataNums = gameData.Split(',');
                var hitData = new HitData
                              {
                                  HitCount = dataNums[0].ToInt32(),
                                  ChargeHitCount = dataNums[1].ToInt32(),
                                  CheckTime = dataNums[2].ToInt32()
                              };
                hitDatas.Add(hitData);
            }
            return hitDatas;
        }

        private void InitAppParam()
        {
            //初始化按钮
            _pointLightButtonItems = ButtonItems.Take(20).ToList();
            for (var i = 20; i < 28; i++)
            {
                _questionLightButtonItems.Add(ButtonItems[i]);
            }
            _beginButtonItem = ButtonItems[28];
            //初始化继电器
            _pointLightRelayItems = RelayItems.Take(20).ToList();
            for (var i = 20; i < 28; i++)
            {
                _questionLightRelayItems.Add(RelayItems[i]);
            }
            for (var i = 28; i < 36; i++)
            {
                _styleLightRelayItems.Add(RelayItems[i]);
            }
            for (var i = 36; i < 46; i++)
            {
                _chargeLightRelayItems.Add(RelayItems[i]);
            }
            _roomLightRelayItem = RelayItems[46];

            RoomLightHelper.RoomRelayLight = _roomLightRelayItem;

            MainUI.BreakThird += MainUI_BreakThird;
            MainUI.BreakUnThird += MainUI_BreakUnThird;
        }

        private void MainUI_BreakUnThird(object sender, EventArgs e)
        {
            _isBreakThird = false;
        }

        private void MainUI_BreakThird(object sender, EventArgs e)
        {
            _isBreakThird = true;
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            InitGameData();
            InitAppParam();
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            while (true)
            {
                for (var i = 0; i < _gameCount; i++)
                {
                    if (_isBreakThird && (i >= 2))
                    {
                        GameCompleted();
                        break;
                    }
                    var count = i + 1;
                    ReBegin:
                    var isBegin = await SendWcfCommandPluginsHelper.InvokerQueryDiaitalSwitchWithAutoUpload(_beginButtonItem, _queryTime);
                    if (isBegin)
                    {
                        await PlayTextMusicFromFirstItem("游戏开始");
                        await PlayMusic0("HitMouse", "P5_mixdown.mp3", "back", 0.2, true);

                        //关闭所有灯
                        await CloseAllLight();

                        await _roomLightRelayItem.Control(true);

                        //await PlayTextMusicFromFirstItem($"第{count}轮游戏开始");
                        //开始击打
                        //重建击打行为数据
                        var hitDatas = GetHitDatas();
                        switch (_hitBuff)
                        {
                            case HitType.Default:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            case HitType.Buff1:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            case HitType.Buff2:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            case HitType.Buff3:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount + 5);
                                break;
                            case HitType.Buff4:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount, true);
                                break;
                            case HitType.Buff5:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            case HitType.Buff6:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            case HitType.Buff7:
                                //全局的背景灯切换
                                RoomLightHelper.Start();
                                await HitTask.HitActionForBuffer7(hitDatas, this, hitDatas[0].ChargeHitCount);
                                //结束背景灯切换
                                await RoomLightHelper.Stop();
                                break;
                            case HitType.Buff8:
                                await HitTask.HitActionForDefault(hitDatas, this, hitDatas[0].ChargeHitCount);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        await PauseMusic0("HitMouse", "back");
                        //await PlayTextMusicFromFirstItem($"第{count}轮游戏结束");
                        //选择招式
                        if (i + 1 != _gameCount)
                        {
                            if (_isBreakThird && (i >= 1))
                            {
                                //await PlayTextMusicFromFirstItem("本轮游戏胜利。");
                            }
                            else
                            {
                                await PlayTextMusicFromFirstItem("本轮游戏胜利，请选择下一轮游戏所要启动的招式。");
                            }
                            foreach (var questionLightRelayItem in _questionLightRelayItems)
                            {
                                await questionLightRelayItem.Control(true);
                            }
                            foreach (var styleLightRelayItem in _styleLightRelayItems)
                            {
                                await styleLightRelayItem.Control(true);
                            }
                            for (var j = 0; j < _chargeLightRelayItems.Count; j++)
                            {
                                if (j != _chargeLightRelayItems.Count - 1)
                                {
                                    await _chargeLightRelayItems[j].Control(false);
                                }
                            }
                            await _roomLightRelayItem.Control(false);
                            var hitSelector = _hitTypes.FirstOrDefault(s => s.Item1 == count);
                            _hitBuff = (HitType)Enum.ToObject(typeof(HitType), hitSelector.Item2);

                            var isPressQuestionButton = false;
                            while (!isPressQuestionButton)
                            {
                                if (_isBreakThird && (i >= 1))
                                {
                                    break;
                                }
                                var pressQuestionButtonItem = await SendWcfCommandPluginsHelper.NotificationButtonPressAsyncTask(_questionLightButtonItems, _queryTime);
                                if (pressQuestionButtonItem != null)
                                {
                                    await _chargeLightRelayItems[_chargeLightRelayItems.Count - 1].Control(false);
                                    await PlayMusic0("HitMouse", hitSelector.Item3, "buff");
                                    await Task.Delay(2000);
                                    isPressQuestionButton = true;
                                }
                            }
                            //var pressQuestionButtonItemIndex = _questionLightButtonItems.IndexOf(pressQuestionButtonItem);
                            for (var j = 0; j < _questionLightRelayItems.Count; j++)
                            {
                                await _questionLightRelayItems[j].Control(false);
                            }

                            //切换对应招式行为
                            //while (true)
                            //{
                            //    var tick = DateTime.Now.Ticks;
                            //    var random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                            //    var buttonIndex = random.Next(1, 8);
                            //    var hitType = (HitType)Enum.Parse(typeof(HitType), buttonIndex.ToString());
                            //    _hitBuff = hitType;
                            //}
                            var hitStyleData = _hitStyleDatas.FirstOrDefault(s => s.QuestionLightButtonIndex == _hitBuff.ToInt32());
                            if (hitStyleData == null)
                            {
                                throw new ArgumentNullException();
                            }

                            if (_isBreakThird && (i >= 1))
                            {
                                foreach (var hitStyleDataItem in _hitStyleDatas)
                                {
                                    foreach (var i1 in hitStyleDataItem.LightIndexs)
                                    {
                                        var index = i1 - 1;
                                        await _styleLightRelayItems[index].Control(false);
                                    }
                                }
                            }
                            else
                            {
                                var usedStyleLightRelayItemIndexs = new List<int>();
                                foreach (var styleLightIndex in hitStyleData.LightIndexs)
                                {
                                    var index = styleLightIndex - 1;
                                    usedStyleLightRelayItemIndexs.Add(index);
                                    await _styleLightRelayItems[index].Control(true);
                                }
                                //关闭不需要的招式灯
                                await CloseStyleLights(usedStyleLightRelayItemIndexs);
                                await PlayTextMusicFromFirstItem(_hitBuff.GetEnumAttribute<TextToMusicAttribute>().Text);
                            }
                        }
                        else
                        {
                            GameCompleted();
                        }
                    }
                    else
                    {
                        goto ReBegin;
                    }
                }
                break;
            }
        }

        private async Task CloseAllLight()
        {
            //灭掉充能灯
            foreach (var chargeLightRelayItem in _chargeLightRelayItems)
            {
                await chargeLightRelayItem.Control(false);
            }
            //灭掉问号灯
            foreach (var questionLightButtonItem in _questionLightRelayItems)
            {
                await questionLightButtonItem.Control(false);
            }
            //灭掉招式灯
            //foreach (var styleLightRelayItem in _styleLightRelayItems)
            //{
            //    await styleLightRelayItem.Control(false);
            //}
        }

        private async Task CloseStyleLights(ICollection<int> usedStyleLightIndexs)
        {
            for (var i = 0; i < _styleLightRelayItems.Count; i++)
            {
                if (!usedStyleLightIndexs.Contains(i))
                {
                    await _styleLightRelayItems[i].Control(false);
                }
            }
        }

        private async void GameCompleted()
        {
            //await PlayTextMusicFromFirstItem("梅花桩游戏结束");
            OnGameOver();
        }
    }
}