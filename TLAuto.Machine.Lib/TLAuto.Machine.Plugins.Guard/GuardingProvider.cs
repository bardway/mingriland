// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;

using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Machine.Plugins.HitCheckTask;
using TLAuto.Notification.Contracts;
using TLAuto.Video.Contracts;
#endregion

namespace TLAuto.Machine.Plugins.Guard
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class GuardingProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Guard";

        private const int WaitingTime = 10 * 60 * 1000;

        //private readonly Stopwatch _guardWatch = new Stopwatch();
        private readonly Timer _guardTimer = new Timer(4 * 1000) {AutoReset = false};

        private readonly Timer _leaveTimer = new Timer(6 * 1000) {AutoReset = false};
        private readonly string _musicMinAddress = ConfigurationManager.AppSettings["GuardMusicServiceAddress"];
        private readonly string _newNotificationServiceAddress = ConfigurationManager.AppSettings["GuardNotificationServiceAddress"];
        private readonly Timer _stopTimer = new Timer(7 * 1000) {AutoReset = false};
        private readonly Timer _toguardTimer = new Timer(15 * 1000) {AutoReset = false};
        private readonly string _videoAddress = ConfigurationManager.AppSettings["GuardVideoServiceAddress"];

        private MachineButtonItem _aisleCheck;
        private bool _aisllight = true;

        private MachineButtonItem _failButton;

        private MachineRelayItem _faillight;
        private HitCheckTask.HitCheckTask _hitCheckTask;
        private bool _leaved;
        private MachineButtonItem _listenButton;
        private MachineRelayItem _listenLight;

        private bool _personavailable;
        private MachineButtonItem _roomCheck;
        private bool _roomlight = true;

        private void Init()
        {
            _aisleCheck = ButtonItems[0];
            _roomCheck = ButtonItems[1];
            _listenButton = ButtonItems[2];
            _failButton = ButtonItems[3];

            _listenLight = RelayItems[0];
            _faillight = RelayItems[1];
            _guardTimer.Elapsed += _guardTimer_Elapsed;
            _stopTimer.Elapsed += _stopTimer_Elapsed;
            _leaveTimer.Elapsed += _leaveTimer_Elapsed;
            _toguardTimer.Elapsed += _toguardTimer_Elapsed;
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _listenButton);
        }

        private void _toguardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _leaveTimer.Stop();
            _guardTimer.Start();
        }

        private async void _leaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(30), VideoActionType.Play, _videoAddress);
            await Task.Delay(2 * 1000);
            _toguardTimer.Start();
            CallOnNotification("回来");
            if (!_aisllight)
            {
                await Stop();
                //return;
            }
            _leaved = false;
        }

        private async void _stopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //pause video
            _leaved = false;
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(2), VideoActionType.Play, _videoAddress);
            _stopTimer.Start();
        }

        private async void _guardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(17), VideoActionType.Play, _videoAddress);
            _leaveTimer.Start();
            //await PlayMusic0(SignKey, "leave.wav", "ear");
            CallOnNotification("离开");
            //await Task.Delay(4 * 1000);
            _leaved = true;
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override async void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            CallOnNotification("part 1");
            //Todo:HHC,启动的时候先定位到指定一帧
            await SendWcfCommandPluginsHelper.PlayVideo(SignKey, "guard.mp4", 1, false, _videoAddress);
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(2), VideoActionType.Stop, _videoAddress);

            //Todo:判断是否开始继续执行
            var result = false;
            while (!result)
            {
                var resultStatus = await SendWcfCommandPluginsHelper.GetNotificationStatus("GuardStart", _newNotificationServiceAddress);
                if (resultStatus == AppStatusType.Stop)
                {
                    result = true;
                }
                else
                {
                    await Task.Delay(2000);
                }
            }
            CallOnNotification("part 2");
            Task.Factory.StartNew(Guarding);
        }
        #endregion

        private async void ResetLight(bool light)
        {
            await _faillight.Control(light);
            await _listenLight.Control(light);
        }

        private async void Guarding()
        {
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(15), VideoActionType.Pause, _videoAddress);
            _toguardTimer.Start();
            ResetLight(false);
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
                            var switchnumber = result.SwitchItem.SwitchNumber;
                            if (switchnumber == _aisleCheck.Number)
                            {
                                _aisllight = result.SwitchItem.SwitchStatus == SwitchStatus.NO;
                            }
                            if (switchnumber == _roomCheck.Number)
                            {
                                _roomlight = result.SwitchItem.SwitchStatus == SwitchStatus.NO;
                            }
                            CallOnNotification($"aisle = {_aisllight} --- room = {_roomlight}");
                            if (_aisllight && _roomlight && !_leaved)
                            {
                                //jump to normal loop
                                _stopTimer.Stop();
                                await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(15), VideoActionType.Play, _videoAddress);
                                await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(15), VideoActionType.Pause, _videoAddress);
                                _toguardTimer.Start();
                                await _faillight.Control(false);
                                if (!_personavailable)
                                {
                                    await _listenLight.Control(false);
                                }
                                continue;
                            }

                            if (result.SwitchItem.SwitchStatus == SwitchStatus.NO)
                            {
                                continue;
                            }

                            if (!_leaved)
                            {
                                if ((switchnumber == _aisleCheck.Number) || ((switchnumber == _roomCheck.Number) && !_personavailable))
                                {
                                    //jump to stop, and pause
                                    await Stop();
                                }
                            }
                            else
                            {
                                if (switchnumber == _roomCheck.Number)
                                {
                                    await _listenLight.Control(true);
                                    await _faillight.Control(false);
                                    _personavailable = true;
                                }
                            }

                            if ((switchnumber == _failButton.Number) && _faillight.IsNo)
                            {
                                CallOnNotification("偷听失败");
                                await PlayMusic0(SignKey, "fail.wav", "ear", musicAddress: _musicMinAddress);
                                await Stop(false);
                            }
                            if ((switchnumber == _listenButton.Number) && _listenLight.IsNo)
                            {
                                CallOnNotification("偷听成功");
                                await PlayMusic0(SignKey, "success.wav", "ear", musicAddress: _musicMinAddress);
                                await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(0), VideoActionType.Play, _videoAddress);
                                await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(0), VideoActionType.Pause, _videoAddress);
                                OnGameOver();
                                return;
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

        private async Task Stop(bool say = true)
        {
            _personavailable = false;
            await SendWcfCommandPluginsHelper.ChangeFrame(TimeSpan.FromSeconds(2), VideoActionType.Play, _videoAddress);
            _stopTimer.Start();
            _guardTimer.Stop();
            _leaveTimer.Stop();
            _toguardTimer.Stop();
            if (say)
            {
                await PlayMusic0(SignKey, "stop.wav", "ear");
            }
            CallOnNotification("禁止入内");
            await _faillight.Control(true);
            await _listenLight.Control(false);
        }
    }
}