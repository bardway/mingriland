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

namespace TLAuto.Machine.Plugins.Eyes
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class EyesProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Eyes";
        private readonly Dictionary<MachineButtonItem, MachineRelayItem> _eyebuttons = new Dictionary<MachineButtonItem, MachineRelayItem>();
        private HitCheckTask.HitCheckTask _hitCheckTask;

        private void Init()
        {
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, ButtonItems[0]);
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            _eyebuttons.Add(ButtonItems[0], RelayItems[0]);
            _eyebuttons.Add(ButtonItems[1], RelayItems[1]);
            _eyebuttons.Add(ButtonItems[2], RelayItems[2]);
            _eyebuttons.Add(ButtonItems[3], RelayItems[3]);
            _eyebuttons.Add(ButtonItems[4], RelayItems[4]);
            _eyebuttons.Add(ButtonItems[5], RelayItems[5]);

            Init();
            Task.Factory.StartNew(EyePick);
        }
        #endregion

        private async void EyePick()
        {
            while (true)
            {
                try
                {
                    var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTask, 10 * 60 * 1000);
                    var results = await hitchecktaskasync.InvokeAsync();
                    if (null != results)
                    {
                        foreach (var result in results)
                        {
                            if (result.SwitchItem.SwitchStatus == SwitchStatus.NO)
                            {
                                continue;
                            }
                            var eye = ButtonItems.FirstOrDefault(r => r.Number == result.SwitchItem.SwitchNumber);
                            if (null == eye)
                            {
                                continue;
                            }
                            //_eyeroller[eye] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;
                            var eyebutton = _eyebuttons.FirstOrDefault(e => e.Key.Number == result.SwitchItem.SwitchNumber);
                            if (eyebutton.Value != null)
                            {
                                foreach (var machineRelayItem in _eyebuttons)
                                {
                                    await machineRelayItem.Value.Control(false);
                                }
                                CallOnNotification($"button:{eye.Number} - status:{result.SwitchItem.SwitchStatus} eye:{eyebutton.Value.Number}");
                                await eyebutton.Value.Control(result.SwitchItem.SwitchStatus == SwitchStatus.NC);
                                if (result.SwitchItem.SwitchStatus == SwitchStatus.NC)
                                {
                                    var tick = DateTime.Now.Ticks;
                                    var randomnumber = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)).Next(1, 7);
                                    await PlayMusic0(SignKey, $"e{randomnumber}.wav", "eyesound");
                                }
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
    }
}