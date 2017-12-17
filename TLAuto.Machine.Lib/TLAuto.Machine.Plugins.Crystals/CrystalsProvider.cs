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

namespace TLAuto.Machine.Plugins.Crystals
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class CrystalsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Crystals";
        private readonly Dictionary<MachineButtonItem, int> _ballStatus = new Dictionary<MachineButtonItem, int>();
        private MachineButtonItem _ball1;
        private MachineButtonItem _ball2;
        private MachineButtonItem _ball3;
        private MachineButtonItem _blow;
        private HitCheckTask.HitCheckTask _hitCheckTask;

        private MachineRelayItem _light1;
        private MachineRelayItem _light2;
        private MachineRelayItem _light3;
        private MachineRelayItem _light4;

        private void Init()
        {
            _ball1 = ButtonItems[0];
            _ball2 = ButtonItems[1];
            _ball3 = ButtonItems[2];
            _blow = ButtonItems[3];

            _light1 = RelayItems[0];
            _light2 = RelayItems[1];
            _light3 = RelayItems[2];
            _light4 = RelayItems[3];

            _ballStatus.Add(_blow, -1);
            _ballStatus.Add(_ball1, -1);
            _ballStatus.Add(_ball2, -1);
            _ballStatus.Add(_ball3, -1);

            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _blow);
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(Crystalcheck);
        }
        #endregion

        private async void Crystalcheck()
        {
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
                            if ((result.SwitchItem.SwitchNumber == _ball1.Number) && !_light1.IsNo)
                            {
                                //_ballStatus[_ball1] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;
                                if (result.SwitchItem.SwitchStatus == SwitchStatus.NC)
                                {
                                    _ballStatus[_ball1] = 1;
                                    await PlayMusic0(SignKey, "fire.wav", "crystal");
                                    await _light1.Control(true);
                                }
                            }

                            if ((result.SwitchItem.SwitchNumber == _ball2.Number) && !_light2.IsNo)
                            {
                                //_ballStatus[_ball2] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;
                                if (result.SwitchItem.SwitchStatus == SwitchStatus.NC)
                                {
                                    _ballStatus[_ball2] = 1;
                                    await PlayMusic0(SignKey, "lightning.wav", "crystal");
                                    await _light2.Control(true);
                                }
                            }

                            if ((result.SwitchItem.SwitchNumber == _ball3.Number) && !_light3.IsNo)
                            {
                                //_ballStatus[_ball3] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;
                                if (result.SwitchItem.SwitchStatus == SwitchStatus.NC)
                                {
                                    _ballStatus[_ball3] = 1;
                                    await PlayMusic0(SignKey, "water.wav", "crystal");
                                    await _light3.Control(true);
                                }
                            }

                            if ((result.SwitchItem.SwitchNumber == _blow.Number) && !_light4.IsNo)
                            {
                                _ballStatus[_blow] = result.SwitchItem.SwitchStatus == SwitchStatus.NC ? 1 : 0;
                                if (result.SwitchItem.SwitchStatus == SwitchStatus.NC)
                                {
                                    await PlayMusic0(SignKey, "wind.wav", "crystal");
                                    await _light4.Control(true);
                                }
                            }

                            if (_ballStatus.All(r => 1 == r.Value))
                            {
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