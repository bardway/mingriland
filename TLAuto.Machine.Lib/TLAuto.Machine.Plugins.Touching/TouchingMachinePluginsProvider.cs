// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
#endregion

namespace TLAuto.Machine.Plugins.Touching
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TouchingMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Touching";
        private MachineButtonItem _beginButtonItem;
        private MachineButtonItem _endButtonItem;
        private MachineRelayItem _errorRelayItem;

        private bool _isRepeatPlayMusic;
        private MachineButtonItem _midwayButtonItem;
        private MachineRelayItem _rightRelayItem;

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            _beginButtonItem = ButtonItems[0];
            _midwayButtonItem = ButtonItems[1];
            _endButtonItem = ButtonItems[2];
            _errorRelayItem = RelayItems[0];
            _rightRelayItem = RelayItems[1];
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            //await PlayTextMusicFromFirstItem("游戏开始");
            var start = 0;
            while (true)
            {
                bool isTouchingBeginPoint;
                if (start == 0)
                {
                    isTouchingBeginPoint = true;
                }
                else
                {
                    if (!_isRepeatPlayMusic)
                    {
                        //await PlayTextMusicFromFirstItem("请重新触摸起点开始游戏");
                        //await PauseMusic0(SignKey, "Voice");
                    }
                    _isRepeatPlayMusic = true;
                    isTouchingBeginPoint = await SendWcfCommandPluginsHelper.InvokerQueryDiaitalSwitchWithAutoUpload(_beginButtonItem, 60000);
                }
                if (isTouchingBeginPoint)
                {
                    await _errorRelayItem.Control(false);
                    //await PlayTextMusicFromFirstItem("开始走位");
                    var touchingButtonItem = await SendWcfCommandPluginsHelper.NotificationButtonPressAsyncTask(new List<MachineButtonItem> {_midwayButtonItem, _endButtonItem}, 60000 * 9);
                    if ((touchingButtonItem != null) && (touchingButtonItem.Number == _endButtonItem.Number))
                    {
                        break;
                    }
                    await _errorRelayItem.Control(true);
                    //PlayTextMusicFromFirstItem("");
                    await PauseMusic0(SignKey, "Voice");
                    await PlayMusic0(SignKey, "line2.wav", "Voice");
                    //PlayTextMusicFromFirstItem("请触碰起点，红灯熄灭，开始游戏。");
                    //await Task.Delay(3000);
                    _isRepeatPlayMusic = false;
                }
                await Task.Delay(1000);
                start++;
            }
            GameCompleted();
        }

        private async void GameCompleted()
        {
            await _rightRelayItem.Control(true);
            //await PlayTextMusicFromFirstItem("游戏完成");
            OnGameOver();
        }
    }
}