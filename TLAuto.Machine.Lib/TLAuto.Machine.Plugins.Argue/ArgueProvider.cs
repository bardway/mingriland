// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Machine.Plugins.HitCheckTask;
#endregion

namespace TLAuto.Machine.Plugins.Argue
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ArgueProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Argue";
        private const int WaitingTime = 10 * 60 * 1000;
        private readonly Timer _gameRefutingTimer = new Timer {AutoReset = false};
        private readonly Timer _gameSayingTimer = new Timer {AutoReset = false};
        private readonly List<MachineRelayItem> _life = new List<MachineRelayItem>();
        private readonly Timer _lifeTimer = new Timer(40 * 1000) {AutoReset = false};
        private readonly Dictionary<MachineButtonItem, Tuple<string, int, MachineButtonItem, MachineButtonItem, MachineRelayItem>> _persons = new Dictionary<MachineButtonItem, Tuple<string, int, MachineButtonItem, MachineButtonItem, MachineRelayItem>>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _alive;
        private MachineButtonItem _currentButton;
        private MachineRelayItem _fotoElf;
        private MachineRelayItem _fotoKing;
        private MachineRelayItem _fotoKnight;
        private MachineRelayItem _fotoMage;

        private HitCheckTask.HitCheckTask _hitCheckTask;

        private MachineButtonItem _hitElf;
        private MachineButtonItem _hitKing;
        private MachineButtonItem _hitKnight;
        private MachineButtonItem _hitMage;

        private MachineButtonItem _listenElf;
        private MachineButtonItem _listenKing;
        private MachineButtonItem _listenKnight;
        private MachineButtonItem _listenMage;
        private bool _refuting;
        private bool _saying;

        private void Init()
        {
            _hitKing = ButtonItems[0];
            _listenKing = ButtonItems[1];
            _hitMage = ButtonItems[2];
            _listenMage = ButtonItems[3];
            _hitKnight = ButtonItems[4];
            _listenKnight = ButtonItems[5];
            _hitElf = ButtonItems[6];
            _listenElf = ButtonItems[7];

            _fotoKing = RelayItems[0];
            _fotoMage = RelayItems[1];
            _fotoKnight = RelayItems[2];
            _fotoElf = RelayItems[3];

            _life.Add(RelayItems[4]);
            _life.Add(RelayItems[5]);
            _life.Add(RelayItems[6]);

            _persons.Add(_listenKing, Tuple.Create("argue_king.wav", 38000, _listenKing, _hitKing, _fotoKing));
            _persons.Add(_listenKnight, Tuple.Create("argue_knight.wav", 24000, _listenKnight, _hitKnight, _fotoKnight));
            _persons.Add(_listenMage, Tuple.Create("argue_mage.wav", 43000, _listenMage, _hitMage, _fotoMage));
            _persons.Add(_listenElf, Tuple.Create("argue_elf.wav", 18000, _listenElf, _hitElf, _fotoElf));

            _gameSayingTimer.Elapsed += GameSayingTimerElapsed;
            _gameRefutingTimer.Elapsed += _gameRefutingTimer_Elapsed;
            _lifeTimer.Elapsed += LifeTimerElapsed;
            _alive = true;
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _hitKing);
        }

        private async void _gameRefutingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _refuting = false;
            if (!_alive)
            {
                await PlayMusic0(SignKey, "dead.wav");
            }
        }

        private void LifeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            LifeRest(true);
            _alive = true;
        }

        private void GameSayingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            StopSaying();
        }

        private async void StopSaying()
        {
            if (null != _currentButton)
            {
                await _persons[_currentButton].Item5.Control(false);
            }
            CallOnNotification("stop saying");
            _saying = false;
            _currentButton = null;
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(KnockingRun);
        }
        #endregion

        private async void ListenCheck(MachineButtonItem pressingButton, string say)
        {
            var person = _persons[pressingButton];
            await person.Item5.Control(true);
            CallOnNotification($"{person.Item1} say");
            _currentButton = pressingButton;
            _stopwatch.Reset();
            _stopwatch.Start();
            _saying = true;
            await PlayMusic0(SignKey, person.Item1, say);
            _gameSayingTimer.Interval = person.Item2;
            _gameSayingTimer.Start();
        }

        private async Task<bool> HitCheck(int hitting, string failsay, int rangestart = 0, int rangeend = 0)
        {
            if ((null != _currentButton) && (hitting == _currentButton.Number))
            {
                var person = _persons[_currentButton];
                CallOnNotification($"counter: {_stopwatch.ElapsedMilliseconds}");
                if ((_stopwatch.ElapsedMilliseconds > rangestart) && (_stopwatch.ElapsedMilliseconds < rangeend))
                {
                    CallOnNotification($"{person.Item1} success.{_stopwatch.ElapsedMilliseconds}");
                    await PlayMusic0(SignKey, "argue_truth.wav", "endknight");
                    await _fotoKnight.Control(false);
                    await Task.Delay(15000);
                    OnGameOver();
                    return true;
                }
                CallOnNotification($"{person.Item1} failed. {_stopwatch.ElapsedMilliseconds}");
                await PlayMusic0(SignKey, failsay, failsay);
                await Hurt();
                return false;
            }
            return false;
        }

        private async Task<bool> Hurt()
        {
            var onelife = _life.FirstOrDefault(l => l.IsNo);
            if (null == onelife)
            {
                return false;
            }
            StopSaying();
            await onelife.Control(false);
            if (_life.All(l => !l.IsNo))
            {
                _alive = false;
                _lifeTimer.Start();
                _gameSayingTimer.Stop();
            }
            return true;
        }

        private async void LifeRest(bool life)
        {
            foreach (var machineRelayItem in _life)
            {
                await machineRelayItem.Control(life);
            }
        }

        private async void KnockingRun()
        {
            LifeRest(true);
            await _fotoElf.Control(false);
            await _fotoKing.Control(false);
            await _fotoKnight.Control(false);
            await _fotoMage.Control(false);
            while (true)
            {
                try
                {
                    var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTask, WaitingTime);
                    var results = await hitchecktaskasync.InvokeAsync();
                    if (null != results)
                    {
                        foreach (var result in results)
                        {
                            if ((result.SwitchItem.SwitchStatus == SwitchStatus.NO) || !_alive)
                            {
                                continue;
                            }

                            var switchnumber = result.SwitchItem.SwitchNumber;

                            if ((_currentButton != null) && (_currentButton.Number == _listenKnight.Number) && (switchnumber == _hitKnight.Number))
                            {
                                await PauseMusic0(SignKey, "knightsay");
                                var hitcheck = await StopArgue(8, _listenKnight.Number, "argue_knightwrong.wav", 12 * 1000, 15 * 1000);
                                if (hitcheck)
                                {
                                    OnGameOver();
                                    return;
                                }
                            }
                            if ((_currentButton != null) && (_currentButton.Number == _listenKing.Number) && (switchnumber == _hitKing.Number))
                            {
                                await PauseMusic0(SignKey, "kingsay");
                                await StopArgue(6, _listenKing.Number, "argue_kingwrong.wav");
                            }
                            if ((_currentButton != null) && (_currentButton.Number == _listenMage.Number) && (switchnumber == _hitMage.Number))
                            {
                                await PauseMusic0(SignKey, "magesay");
                                await StopArgue(9, _listenMage.Number, "argue_magewrong.wav");
                            }
                            if ((_currentButton != null) && (_currentButton.Number == _listenElf.Number) && (switchnumber == _hitElf.Number))
                            {
                                await PauseMusic0(SignKey, "elfsay");
                                await StopArgue(8, _listenElf.Number, "argue_elfwrong.wav");
                            }

                            if ((switchnumber == _listenKing.Number) && !_saying && !_refuting)
                            {
                                ListenCheck(_listenKing, "kingsay");
                            }
                            if ((switchnumber == _listenMage.Number) && !_saying && !_refuting)
                            {
                                ListenCheck(_listenMage, "magesay");
                            }
                            if ((switchnumber == _listenKnight.Number) && !_saying && !_refuting)
                            {
                                ListenCheck(_listenKnight, "knightsay");
                            }
                            if ((switchnumber == _listenElf.Number) && !_saying && !_refuting)
                            {
                                ListenCheck(_listenElf, "elfsay");
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

        private async Task<bool> StopArgue(int refutetime, int sayingnumber, string refutefile, int rangestart = 0, int rangeend = 0)
        {
            _gameRefutingTimer.Interval = refutetime * 1000;
            _refuting = true;
            _gameRefutingTimer.Start();
            var checkresult = await HitCheck(sayingnumber, refutefile, rangestart, rangeend);
            return checkresult;
        }
    }
}