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

namespace TLAuto.Machine.Plugins.Arsh
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class ArshProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Arsh";
        private readonly Dictionary<int, MachineButtonItem> _hit = new Dictionary<int, MachineButtonItem>();
        private readonly List<int> _squence1 = new List<int> {0, 1, 1, 2, 2};
        private readonly List<int> _squence2 = new List<int> {1, 2, 1, 2, 0};
        private int _current;
        private HitCheckTask.HitCheckTask _hitCheckTask;

        private void Init()
        {
            _current = 0;
            _hit.Add(0, ButtonItems[0]);
            _hit.Add(1, ButtonItems[1]);
            _hit.Add(2, ButtonItems[2]);
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, ButtonItems[0]);
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(StabCheck);
        }
        #endregion

        private async void StabCheck()
        {
            await SendWcfCommandPluginsHelper.InvokerNotificationForStart("ArshEnd1");
            await SendWcfCommandPluginsHelper.InvokerNotificationForStart("ArshEnd2");
            while (true)
            {
                try
                {
                    var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTask, 60 * 1000);
                    var results = await hitchecktaskasync.InvokeAsync();
                    if (null != results)
                    {
                        foreach (var result in results)
                        {
                            if (result.SwitchItem.SwitchStatus == SwitchStatus.NO)
                            {
                                continue;
                            }
                            var hit = _hit.FirstOrDefault(h => h.Value.Number == result.SwitchItem.SwitchNumber);
                            if (hit.Value != null)
                            {
                                await PlayMusic0(SignKey, "in.wav", "in");
                                var tick = DateTime.Now.Ticks;
                                var randomcount = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)).Next(2, 5);
                                CallOnNotification($"current  count {randomcount} v: {hit.Value}");

                                foreach (var machineRelayItem in RelayItems)
                                {
                                    await machineRelayItem.Control(false);
                                }
                                for (var i = 0; i < randomcount; i++)
                                {
                                    tick = DateTime.Now.Ticks;
                                    var randomnumber = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)).Next(0, 4);
                                    CallOnNotification($"current number {randomnumber}");
                                    await RelayItems[randomnumber].Control(true);
                                }

                                if (hit.Key == _squence1[_current])
                                {
                                    CallOnNotification($"currentstep: {_current}");
                                    _current++;
                                    if (_current == _squence1.Count)
                                    {
                                        foreach (var machineRelayItem in RelayItems)
                                        {
                                            await machineRelayItem.Control(false);
                                        }
                                        await SendWcfCommandPluginsHelper.InvokerNotificationForStop("ArshEnd1");
                                        OnGameOver();
                                        return;
                                    }
                                    continue;
                                }
                                //if (hit.Key == _squence2[_current])
                                //{
                                //    CallOnNotification($"currentstep: {_current}");
                                //    _current++;
                                //    if (_current == _squence2.Count)
                                //    {
                                //        foreach (var machineRelayItem in RelayItems)
                                //        {
                                //            await machineRelayItem.Control(false);
                                //        }
                                //        await SendWcfCommandPluginsHelper.InvokerNotificationForStop("ArshEnd2");
                                //        OnGameOver();
                                //        return;
                                //    }
                                //    continue;
                                //}
                                if (hit.Key == _squence1[0])
                                {
                                    CallOnNotification($"reset to new step1");
                                    _current = 1;
                                    continue;
                                }
                                CallOnNotification("reset step");
                                _current = 0;
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