// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace TLAuto.Machine.Plugins.Maid
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class MaidProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Maid";
        private static readonly Timer MaidTimer = new Timer {AutoReset = false};
        private static readonly Timer Maid1MuteTimer = new Timer(10 * 1000) {AutoReset = false};
        private static readonly Timer Maid2MuteTimer = new Timer(10 * 1000) {AutoReset = false};
        private readonly List<LineButton> _maid1 = new List<LineButton>();
        private readonly List<LineButton> _maid2 = new List<LineButton>();
        private readonly List<Step> _steps = new List<Step>();
        private Step _currentStep;
        private LineButton _greenFace;

        private HitCheckTask.HitCheckTask _hitCheckTask;
        private LineButton _maid1Breast1;
        private LineButton _maid1Breast2;
        private LineButton _maid1Eye1;
        private LineButton _maid1Eye2;
        private MachineRelayItem _maid1Light;
        private bool _maid1Mute;
        private LineButton _maid1Oxter1;
        private LineButton _maid1Oxter2;
        private LineButton _maid2Breast1;
        private LineButton _maid2Breast2;
        private LineButton _maid2Eye1;
        private LineButton _maid2Eye2;
        private MachineRelayItem _maid2Light;
        private bool _maid2Mute;
        private LineButton _maid2Oxter1;
        private LineButton _maid2Oxter2;
        private LineButton _redFace;
        private bool _saying;

        private int _step;

        private Step _step1;

        private void Init()
        {
            _maid1Eye1 = new LineButton(ButtonItems[0]);
            _maid1Eye2 = new LineButton(ButtonItems[1]);
            _maid1Breast1 = new LineButton(ButtonItems[2]);
            _maid1Breast2 = new LineButton(ButtonItems[3]);
            _maid1Oxter1 = new LineButton(ButtonItems[4]);
            _maid1Oxter2 = new LineButton(ButtonItems[5]);

            _maid2Eye1 = new LineButton(ButtonItems[6]);
            _maid2Eye2 = new LineButton(ButtonItems[7]);
            _maid2Breast1 = new LineButton(ButtonItems[8]);
            _maid2Breast2 = new LineButton(ButtonItems[9]);
            _maid2Oxter1 = new LineButton(ButtonItems[10]);
            _maid2Oxter2 = new LineButton(ButtonItems[11]);
            _greenFace = new LineButton(ButtonItems[12]);
            _redFace = new LineButton(ButtonItems[13]);

            _maid1Light = RelayItems[0];
            _maid2Light = RelayItems[1];

            _maid1.Add(_maid1Eye1);
            _maid1.Add(_maid1Eye2);
            _maid1.Add(_maid1Breast1);
            _maid1.Add(_maid1Breast2);
            _maid1.Add(_maid1Oxter1);
            _maid1.Add(_maid1Oxter2);

            _maid2.Add(_maid2Eye1);
            _maid2.Add(_maid2Eye2);
            _maid2.Add(_maid2Breast1);
            _maid2.Add(_maid2Breast2);
            _maid2.Add(_maid2Oxter1);
            _maid2.Add(_maid2Oxter2);

            _step = 1;
            CreatStep();
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _redFace.MachineButtonItem);
            MaidTimer.Elapsed += MaidTimer_Elapsed;
            Maid1MuteTimer.Elapsed += Maid1MuteTimer_Elapsed;
            Maid2MuteTimer.Elapsed += Maid2MuteTimer_Elapsed;
        }

        private async void Maid2MuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_maid2Light.IsNo)
            {
                await _maid2Light.Control(true);
                _maid2Mute = false;
            }
        }

        private async void Maid1MuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_maid1Light.IsNo)
            {
                await _maid1Light.Control(true);
                _maid1Mute = false;
            }
        }

        private void MaidTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _saying = false;
        }

        private void CreatStep()
        {
            _step1 = new Step(1, "maid2-oxter.wav", 7000, new List<LineButton> {_maid2Oxter1, _maid2Oxter2}); //11

            _steps.Add(_step1);
            _steps.Add(new Step(2, "grennface2.wav", 9000, new List<LineButton> {_greenFace})); //13
            _steps.Add(new Step(3, "redface2.wav", 7000, new List<LineButton> {_redFace})); //14
            _steps.Add(new Step(4, "maid2-redface.wav", 6000, new List<LineButton> {_maid2Eye1, _maid2Eye2})); //7
            _steps.Add(new Step(5, "redface3.wav", 7000, new List<LineButton> {_redFace})); //14
            _steps.Add(new Step(6, "grennface3.wav", 15000, new List<LineButton> {_greenFace})); //13
            _steps.Add(new Step(7, "maid1-itch1.wav", 2000, new List<LineButton> {_maid1Oxter1, _maid1Oxter2})); //5
            _steps.Add(new Step(8, "maid1-itch2.wav", 2000, new List<LineButton> {_maid1Oxter1, _maid1Oxter2})); //5
            _steps.Add(new Step(9, "maid1-truth.wav", 8000, new List<LineButton> {_maid1Oxter1, _maid1Oxter2})); //5
        }

        private async void Check(int pressingNumber, Step currentstepobj)
        {
            if (null == currentstepobj)
            {
                currentstepobj = _step1;
            }

            if (currentstepobj.ButtonItems.All(o => o.MachineButtonItem.Number != pressingNumber) && !_saying)
            {
                if (pressingNumber == _greenFace.MachineButtonItem.Number)
                {
                    if (await RepeatClick(_greenFace))
                    {
                        return;
                    }
                    await WrongClick(3, "greenface1.wav");
                    return;
                }
                if (pressingNumber == _redFace.MachineButtonItem.Number)
                {
                    if (await RepeatClick(_redFace))
                    {
                        return;
                    }
                    await WrongClick(4, "redface1.wav");
                    return;
                }
                if (!_maid2Mute)
                {
                    if ((pressingNumber == _maid2Oxter1.MachineButtonItem.Number) || (pressingNumber == _maid2Oxter2.MachineButtonItem.Number))
                    {
                        if (await RepeatClick(_maid2Oxter2) || await RepeatClick(_maid2Oxter2))
                        {
                            return;
                        }
                        await WrongClick(2, "maid1-itch1.wav");
                        return;
                    }
                    if ((pressingNumber == _maid2Eye1.MachineButtonItem.Number) || (pressingNumber == _maid2Eye2.MachineButtonItem.Number))
                    {
                        if (await RepeatClick(_maid2Eye1) || await RepeatClick(_maid2Eye2))
                        {
                            return;
                        }
                        await WrongClick(4, "maid2-eye.wav");
                        return;
                    }
                }
                if (!_maid1Mute)
                {
                    if ((pressingNumber == _maid1Oxter1.MachineButtonItem.Number) || (pressingNumber == _maid1Oxter2.MachineButtonItem.Number))
                    {
                        if (await RepeatClick(_maid1Oxter1) || await RepeatClick(_maid1Oxter2))
                        {
                            return;
                        }
                        await WrongClick(2, "maid1-itch1.wav");
                        return;
                    }
                    if ((pressingNumber == _maid1Eye1.MachineButtonItem.Number) || (pressingNumber == _maid1Eye2.MachineButtonItem.Number))
                    {
                        if (await RepeatClick(_maid1Eye1) || await RepeatClick(_maid1Eye2))
                        {
                            return;
                        }
                        await WrongClick(3, "maid1-eye.wav");
                        return;
                    }
                }
            }
            if (currentstepobj.ButtonItems.Any(r => pressingNumber == r.MachineButtonItem.Number))
            {
                if (_maid2.Any(m => m.MachineButtonItem.Number == pressingNumber) && ((_step == 1) || (_step == 4)) && _maid2Mute)
                {
                    return;
                }
                if (_maid1.Any(m => m.MachineButtonItem.Number == pressingNumber) && (_step > 6) && _maid1Mute)
                {
                    return;
                }
                MaidTimer.Interval = currentstepobj.Duration;
                _saying = true;
                await PlayMusic0(SignKey, currentstepobj.Voice, "correctclick");
                var stepobj = _steps.Where(s => s.StepNo == _step);
                foreach (var step in stepobj)
                {
                    foreach (var buttonItem in step.ButtonItems)
                    {
                        buttonItem.Voice = currentstepobj.Voice;
                        buttonItem.Duration = currentstepobj.Duration;
                    }
                }
                MaidTimer.Start();
                _step++;
                CallOnNotification("currentStep: " + _step);
                if (_step > 9)
                {
                    OnGameOver();
                    return;
                }
                var next = _steps.FirstOrDefault(s => s.StepNo == _step);
                _currentStep = next;
            }
            else
            {
                _saying = false;
            }
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(MaidTalk);
        }
        #endregion

        private async void MaidTalk()
        {
            await _maid1Light.Control(true);
            await _maid2Light.Control(true);
            while (true)
            {
                try
                {
                    var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTask);
                    var results = await hitchecktaskasync.InvokeAsync();
                    if (null != results)
                    {
                        foreach (var result in results)
                        {
                            if (result.SwitchItem.SwitchStatus == SwitchStatus.NO)
                            {
                                continue;
                            }
                            if (_saying)
                            {
                                continue;
                            }
                            if (((result.SwitchItem.SwitchNumber == _maid1Breast1.MachineButtonItem.Number) || (result.SwitchItem.SwitchNumber == _maid1Breast2.MachineButtonItem.Number)) && !_maid1Mute)
                            {
                                await PlayMusic0(SignKey, "maid1-rude.wav", "wrongclick");
                                CallOnNotification("maid1 down");
                                await _maid1Light.Control(false);
                                _maid1Mute = true;
                                Maid1MuteTimer.Start();
                                continue;
                            }

                            if (((result.SwitchItem.SwitchNumber == _maid2Breast1.MachineButtonItem.Number) || (result.SwitchItem.SwitchNumber == _maid2Breast2.MachineButtonItem.Number)) && !_maid2Mute)
                            {
                                await PlayMusic0(SignKey, "maid2-rude.wav", "wrongclick");
                                CallOnNotification("maid2 down");
                                await _maid2Light.Control(false);
                                _maid2Mute = true;
                                Maid2MuteTimer.Start();
                                continue;
                            }
                            CallOnNotification($"hitting: {result.SwitchItem.SwitchNumber}");
                            Check(result.SwitchItem.SwitchNumber, _currentStep);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnNotification(new NotificationEventArgs(ex.Message));
                }
            }
        }

        private async Task WrongClick(int duration, string wrongfile)
        {
            MaidTimer.Interval = duration;
            _saying = true;
            await PlayMusic0(SignKey, wrongfile, "wrongclick");
            MaidTimer.Start();
            CallOnNotification("wrong click, currentStep: " + _step);
        }

        private async Task<bool> RepeatClick(LineButton button)
        {
            if (!string.IsNullOrEmpty(button.Voice) && (button.Duration > 0))
            {
                MaidTimer.Interval = button.Duration;
                _saying = true;
                await PlayMusic0(SignKey, button.Voice, "correctclick");
                MaidTimer.Start();
                return true;
            }
            return false;
        }
    }
}