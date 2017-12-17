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

namespace TLAuto.Machine.Plugins.Runes
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class RunesProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Runes";
        private readonly Dictionary<MachineButtonItem, int> _runes = new Dictionary<MachineButtonItem, int>();
        private readonly List<MachineButtonItem> _runeson = new List<MachineButtonItem>();
        private HitCheckTask.HitCheckTask _hitCheckTask;
        private MachineRelayItem _light;

        private MachineButtonItem _rune1;
        private MachineButtonItem _rune2;
        private MachineButtonItem _rune3;

        private void Init()
        {
            //17,28,27
            _rune1 = ButtonItems[0];
            _rune2 = ButtonItems[7];
            _rune3 = ButtonItems[8];

            _light = RelayItems[0];

            foreach (var machineButtonItem in ButtonItems)
            {
                _runes.Add(machineButtonItem, 0);
            }

            _runeson.Add(_rune1);
            _runeson.Add(_rune2);
            _runeson.Add(_rune3);

            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _rune1);
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(RunePick);
        }
        #endregion

        private async void RunePick()
        {
            await _light.Control(true);
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
                            var rune = ButtonItems.FirstOrDefault(r => r.Number == result.SwitchItem.SwitchNumber);
                            if (null == rune)
                            {
                                continue;
                            }
                            _runes[rune] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;

                            CallOnNotification($"rune: {result.SwitchItem.SwitchNumber} status: {result.SwitchItem.SwitchStatus}");
                            await PlayMusic0(SignKey, "touch.wav", "touching");
                            var runeon = _runes.Where(r => _runeson.Contains(r.Key));
                            var runeoff = _runes.Where(r => !_runeson.Contains(r.Key));

                            if (runeon.All(r => 1 == r.Value) && runeoff.All(r => 0 == r.Value))
                            {
                                await PauseMusic0(SignKey, "touching");
                                await PlayMusic0(SignKey, "touchsuccess.wav", "touchover");
                                await Task.Delay(4000);
                                await _light.Control(false);
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
    }
}