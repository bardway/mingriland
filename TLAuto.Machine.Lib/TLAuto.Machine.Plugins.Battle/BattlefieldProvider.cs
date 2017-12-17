// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Machine.Plugins.HitCheckTask;
#endregion

namespace TLAuto.Machine.Plugins.Battle
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class BattlefieldProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Battle";

        private static readonly BattleEnemyTimer BattleEnemyTimer = new BattleEnemyTimer(SignKey);
        private readonly List<MachineButtonItem> _allHits = new List<MachineButtonItem>();
        private readonly List<HitCheckTask.HitCheckTask> _hitCheckTasks = new List<HitCheckTask.HitCheckTask>();
        private readonly List<MachineRelayItem> _life = new List<MachineRelayItem>();
        private bool _alive;
        private int _currentGame;
        private int _currentRound;
        private volatile bool _isBreakSencond;
        private volatile bool _isBreakThree;
        private MachineButtonItem _startButton;
        private int _totalGame;
        private int _totalRound;

        private void Init()
        {
            MainUI.BreakSecond += delegate { _isBreakSencond = true; };
            MainUI.UnbreakSecond += delegate { _isBreakSencond = false; };
            MainUI.BreakThird += delegate { _isBreakThree = true; };
            MainUI.UnbreakThird += delegate { _isBreakThree = false; };

            BattleEnemyTimer.MachineMusicItems = MusicItems;
            _startButton = ButtonItems[0];
            _life.Add(RelayItems[32]);
            _life.Add(RelayItems[31]);
            _life.Add(RelayItems[30]);
            var buttongroup = ButtonItems.GroupBy(b => b.DeviceNumber).ToDictionary(k => k.Key, v => v.Select(vk => vk).ToList());
            foreach (var buttons in buttongroup)
            {
                var button = buttons.Value.FirstOrDefault();
                if (null == button)
                {
                    continue;
                }
                var hitchecktask = new HitCheckTask.HitCheckTask(SignKey, button);
                _hitCheckTasks.Add(hitchecktask);
            }
            for (var i = 1; i < 31; i++)
            {
                _allHits.Add(ButtonItems[i]);
                Enemies.LoadEnemy(i, ButtonItems[i], RelayItems[i - 1]);
            }
            Enemies.ArrangeEnemey();
            _totalGame = Enemies.GetGamecount();
        }

        private async Task<bool> Hurt()
        {
            if (_currentGame == 2)
            {
                foreach (var machineRelayItem in _life)
                {
                    await machineRelayItem.Control(false);
                }
                CallOnNotification("DEAD xxx");
            }
            else
            {
                var onelife = _life.FirstOrDefault(l => l.IsNo);
                if (null == onelife)
                {
                    return false;
                }
                CallOnNotification("掉血xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                await onelife.Control(false);
            }
            await PlayMusic0(SignKey, "al4.1.wav", "al4.1.wav");
            //var counter = _life.Count(l => !l.IsNo);
            //switch (counter)
            //{
            //    case 1:
            //        await PlayMusic0(SignKey, "al4.1.wav", "al4.1.wav");
            //        break;
            //    case 2:
            //        await PlayMusic0(SignKey, "al4.2.wav", "al4.2.wav");
            //        break;
            //    case 3:
            //        await PlayMusic0(SignKey, "al4.3.wav", "al4.3.wav");
            //        break;
            //}
            //if (_life.All(l => !l.IsNo))
            //{
            //    _alive = false;
            //    _currentRound = 0;
            //    //_roundTimer.Stop();
            //    BattleEnemyTimer.Dispose();
            //    Enemies.RestBattle();
            //    await PauseMusic0(SignKey, "round");
            //    return false;
            //}
            //BattleEnemyTimer.Restart();
            return true;
        }

        private async void RestLife()
        {
            foreach (var machineRelayItem in _life)
            {
                await machineRelayItem.Control(true);
            }
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(BattleOn);
        }
        #endregion

        private async void BattleOn()
        {
            RestLife();
            Enemies.RestBattle();
            while (true)
            {
                try
                {
                    var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTasks, 15 * 60 * 1000);
                    var results = await hitchecktaskasync.InvokeAsync();
                    if (null != results)
                    {
                        foreach (var result in results)
                        {
                            if (null == result)
                            {
                                CallOnNotification("empty button message");
                                continue;
                            }
                            if (result.SwitchItem.SwitchStatus == SwitchStatus.NO)
                            {
                                continue;
                            }
                            var switchnumber = result.SwitchItem.SwitchNumber;
                            if ((switchnumber == _startButton.Number) && (result.DeviceNumber == _startButton.DeviceNumber) && !_alive)
                            {
                                _alive = true;
                                RestLife();
                                Enemies.RestBattle();
                                BattleEnemyTimer.Hurting -= BattleEnemyTimerHurting;
                                BattleEnemyTimer.Hurting += BattleEnemyTimerHurting;
                                BattleEnemyTimer.Finishing -= BattleEnemyTimerFinishing;
                                BattleEnemyTimer.Finishing += BattleEnemyTimerFinishing;
                                _currentRound = 0;
                                Battling();
                                //_roundTimer = new Timer(2 * 60 * 1 * 1000) { AutoReset = false };
                                //_roundTimer.Start();
                                continue;
                            }
                            if (Enemies.CheckIfOff(result.DeviceNumber, result.SwitchItem.SwitchNumber))
                            {
                                continue;
                            }
                            if (_allHits.Any(b => (b.Number == result.SwitchItem.SwitchNumber) && (b.DeviceNumber == result.DeviceNumber)))
                            {
                                var tick = DateTime.Now.Ticks;
                                var randomnumber = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)).Next(1, 5);
                                var hitsound = $"hit{randomnumber}.wav";
                                await PlayMusic0(SignKey, hitsound, hitsound);
                                CallOnNotification($"hit : {result.SwitchItem.SwitchNumber} - {result.DeviceNumber}");
                                ButtonHit(BattleEnemyTimer, result.SwitchItem.SwitchNumber, result.DeviceNumber);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnNotification(new NotificationEventArgs(ex.Message));
                }
            }
        }

        private static async void ButtonHit(BattleEnemyTimer battleEnemyTimer, int switchnumber, int devicenumber)
        {
            var currentHits = battleEnemyTimer.CurrentHits();
            var hit = currentHits?.FirstOrDefault(h => (h.Hitter.Number == switchnumber) && (h.Hitter.DeviceNumber == devicenumber));
            if (hit != null)
            {
                await battleEnemyTimer.Hit(hit);
            }
        }

        private async void Battling()
        {
            if (!_alive)
            {
                return;
            }
            var buttons = Enemies.DeployTroop(_currentGame, _currentRound);
            switch (_currentGame)
            {
                case 0:
                    switch (_currentRound)
                    {
                        case 0:
                            await PlayMusic0(SignKey, "al1.1.wav", "al11");
                            await Task.Delay(8000);
                            await PlayMusic0(SignKey, "battlebgm.mp3", "round", 0.5, true);
                            break;
                        case 1:
                            await PlayMusic0(SignKey, "al1.2.wav", "al12");
                            break;
                        case 2:
                            await PlayMusic0(SignKey, "al1.3.wav", "al13");
                            break;
                    }
                    break;
                case 1:
                    switch (_currentRound)
                    {
                        case 0:
                            await PlayMusic0(SignKey, "al2.1.wav", "al21");
                            await Task.Delay(4000);
                            await PlayMusic0(SignKey, "battlebgm.mp3", "round", 0.5, true);
                            break;
                        case 1:
                            await PlayMusic0(SignKey, "al2.2.wav", "al22");
                            break;
                        case 3:
                            await PlayMusic0(SignKey, "al2.3.wav", "al23");
                            break;
                    }
                    break;
                case 2:
                    switch (_currentRound)
                    {
                        case 0:
                            await PlayMusic0(SignKey, "attack_voice.wav", "round31");
                            await Task.Delay(4000);
                            await PlayMusic0(SignKey, "attack_bgm.wav", "round", 0.5, true);
                            break;
                    }

                    break;
            }
            _totalRound = Enemies.GetRoundcount(_currentGame);
            if (!_alive)
            {
                return;
            }
            BattleEnemyTimer.LoadButtons(buttons.Item1, buttons.Item2);
            await BattleEnemyTimer.GameStart(1);
        }

        private async void BattleEnemyTimerFinishing(object sender, EventArgs e)
        {
            if (!_alive)
            {
                return;
            }
            _currentRound++;
            CallOnNotification($"第{_currentGame}场，第{_currentRound}轮");
            if (_currentRound == _totalRound)
            {
                var gamestage = $"{_currentGame}{_currentRound}";
                CallOnNotification($"回合结束，game: {_currentGame} round: {_currentRound} - {gamestage}");
                switch (gamestage)
                {
                    case "04":
                        await PlayMusic0(SignKey, "al1.4.wav", "al14");
                        await PauseMusic0(SignKey, "round");
                        break;
                    case "15":
                        await PlayMusic0(SignKey, "al2.4.wav", "al24");
                        await PauseMusic0(SignKey, "round");
                        if (_isBreakThree || _isBreakSencond)
                        {
                            await PauseMusic0(SignKey, "round");
                            OnGameOver();
                        }
                        break;
                    case "22":
                        await PlayMusic0(SignKey, "al3.2.wav", "al32");
                        await PauseMusic0(SignKey, "round");
                        OnGameOver();
                        break;
                }

                _currentRound = 0;
                _alive = false;
                _currentGame++;
                return;
            }
            if (_currentGame == _totalGame)
            {
                //游戏结束
                //await PlayMusic0(SignKey, "al5.1.wav", "battlend");
                await PauseMusic0(SignKey, "round");
                OnGameOver();
                return;
            }
            Battling();
        }

        private async void BattleEnemyTimerHurting(object sender, EventArgs e)
        {
            if (!_alive)
            {
                return;
            }
            await Hurt();
        }
    }
}