// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;

using Timer = System.Timers.Timer;
#endregion

namespace TLAuto.Machine.Plugins.Battle
{
    public sealed class BattleEnemyTimer : IDisposable
    {
        private static int _waitingTime;
        private readonly Stopwatch _gameCouting = new Stopwatch();
        private readonly Timer _gameSingleTimer = new Timer {AutoReset = false};
        private readonly string _sign;
        private List<HitButon> _currentButtons;
        private int _currentRound;
        private List<List<HitButon>> _hitbuttons;
        private HitButon _hittedbutton;
        private bool _isstop;
        private int _totalRound;

        public BattleEnemyTimer(string sign)
        {
            _sign = sign;
        }

        public List<MachineMusicItem> MachineMusicItems { get; set; }

        #region Implementation of IDisposable
        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        public void Dispose()
        {
            GameOver(false);
        }
        #endregion

        public void LoadButtons(int interval, List<List<HitButon>> buttons)
        {
            _waitingTime = interval;
            _totalRound = buttons.Count;
            _hitbuttons = buttons;
            _currentRound = 0;
            _currentButtons = null;
            _gameSingleTimer.Elapsed -= _gameSingleTimer_Tick;
            _gameSingleTimer.Elapsed += _gameSingleTimer_Tick;
            ThreadPool.QueueUserWorkItem(async _ => await Checking());
            WriteLog($"load buttons: {_totalRound}");
        }

        public event EventHandler Hurting;

        public event EventHandler Finishing;

        private async void _gameSingleTimer_Tick(object sender, EventArgs e)
        {
            await Checking();
        }

        private async Task Checking(bool immediatly = false)
        {
            if (_currentRound >= _totalRound)
            {
                OnFinishing();
                return;
            }
            _currentButtons = _hitbuttons[_currentRound];
            if (_isstop)
            {
                return;
            }
            if (_gameCouting.ElapsedMilliseconds == 0)
            {
                PrepareHitting();
            }
            if (((_gameCouting.ElapsedMilliseconds >= _waitingTime * 1000) && !_isstop) || immediatly)
            {
                WriteLog($"current time: {_gameCouting.ElapsedMilliseconds}, threadid: {Thread.CurrentThread.ManagedThreadId}");

                if (_currentButtons.Any(b => b.Hiited.HasValue && !b.Hiited.Value) && _currentButtons.Any(b => b.Light.IsNo))
                {
                    var buttons = _currentButtons.Select(b => b.ButtonId).ToList();
                    WriteLog($"{_gameCouting.ElapsedMilliseconds} : 掉血熄灭, -> {string.Join(",", buttons)}");
                    foreach (var currentButton in _currentButtons)
                    {
                        await LightControl(currentButton.Light, false);
                    }
                    //Restart();
                    //_gameCouting.Reset();
                    //_gameCouting.Stop();
                    //_gameSingleTimer.Stop();
                    //OnHurting();
                    _currentRound++;
                    Restart();
                    return;
                }
                if (_currentButtons.Any(b => b.Hiited.HasValue && b.Hiited.Value))
                {
                    foreach (var currentButton in _currentButtons)
                    {
                        currentButton.ResetHitting();
                    }
                    _currentRound++;
                }
                if (PrepareHitting())
                {
                    return;
                }
            }
            if (_isstop)
            {
                return;
            }

            _gameSingleTimer.Start();
        }

        private bool PrepareHitting()
        {
            if (_currentRound >= _totalRound)
            {
                OnFinishing();
                return true;
            }
            _currentButtons = _hitbuttons[_currentRound];
            var buttons = _currentButtons.Select(b => b.ButtonId).ToList();
            var sounds = _currentButtons.Select(b => b.Sound).Distinct();
            Parallel.ForEach(sounds, b => PlayMusic0(b, b));
            foreach (var currentButton in _currentButtons)
            {
                if (currentButton.Hiited.HasValue && currentButton.Hiited.Value)
                {
                    continue;
                }
                // play currentButton.Sound
                // currentButton.Sound

                //if (await currentButton.LightOn())
                {
                    currentButton.LightOn();
                    currentButton.ResetHitting();
                }
            }

            WriteLog($"开始第{_currentRound}批, -> {string.Join(",", buttons)}");
            Restart();
            return false;
        }

        private static async Task LightControl(MachineRelayItem light, bool of)
        {
            await light.Control(of);
        }

        public List<HitButon> CurrentHits()
        {
            return _currentButtons;
        }

        public async Task Hit(HitButon button)
        {
            if (null == _currentButtons)
            {
                return;
            }
            if (button == _hittedbutton)
            {
                return;
            }
            _hittedbutton = button;
            var hitter = _currentButtons.FirstOrDefault(b => (b.Hitter.Number == button.Hitter.Number) && (b.Hitter.DeviceNumber == button.Hitter.DeviceNumber));
            var task = hitter;
            if (task != null)
            {
                await task.Hit();
                WriteLog($"{_gameCouting.ElapsedMilliseconds} : {button.ButtonId}, hitted <<<<<<<<");
                //await Checking();
            }
        }

        public async Task GameStart(int delaytime)
        {
            await Task.Delay(delaytime * 1000);
            _isstop = false;
            ThreadPool.QueueUserWorkItem(async _ => await Checking());
            _gameSingleTimer.Start();
            _gameCouting.Start();
        }

        private void GameOver(bool stop)
        {
            _isstop = stop;
            Stop();
        }

        private void Restart()
        {
            WriteLog($"{_gameCouting.ElapsedMilliseconds} : 重置计时");
            _isstop = false;
            _gameCouting.Reset();
            _gameCouting.Start();
            _gameSingleTimer.Start();
        }

        private void Stop()
        {
            WriteLog($"{_gameCouting.ElapsedMilliseconds} : 停止计时");
            _isstop = true;
            _gameSingleTimer.Stop();
            _gameCouting.Reset();
            _gameCouting.Stop();
        }

        private void OnHurting()
        {
            Hurting?.Invoke(this, EventArgs.Empty);
        }

        private void OnFinishing()
        {
            Finishing?.Invoke(this, EventArgs.Empty);
        }

        private async void PlayMusic0(string fileName, string musicKey = null, double volume = 1, bool isRepeat = false, string musicAddress = null)
        {
            var musicPathBase = @"C:\Program Files\StartGateServer\TLAuto.Machine\" + _sign + @"\MachinePlugins\Music";
            await SendWcfCommandPluginsHelper.InvokerMusic(_sign + (musicKey.IsNullOrEmpty() ? "" : musicKey), Path.Combine(musicPathBase, fileName), volume, isRepeat, musicAddress ?? MachineMusicItems[0].ServiceAddress);
        }

        private void WriteLog(string message)
        {
            Debug.WriteLine($"{DateTime.Now} : {message}");
        }
    }
}