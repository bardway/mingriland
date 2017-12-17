// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.HitCheckTask;
#endregion

namespace TLAuto.Machine.Plugins.Knocking
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class KnockingProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Knocking";
        private const int KnockTimes = 11;
        private const int WaitingTime = 5000;
        private HitCheckTask.HitCheckTask _hitCheckTask;
        private MachineButtonItem _knocker;

        private void Init()
        {
            _knocker = ButtonItems[0];
            _hitCheckTask = new HitCheckTask.HitCheckTask(SignKey, _knocker);
        }

        #region Overrides of MachinePluginsProvider<Main>
        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(KnockingRun);
        }
        #endregion

        private async void KnockingRun()
        {
            var knockingcount = 0;
            while (true)
            {
                var hitchecktaskasync = new HitCheckTaskAsync(_hitCheckTask, WaitingTime);
                var results = await hitchecktaskasync.InvokeAsync();
                if (null != results)
                {
                    foreach (var switchItemWithDeviceNumber in results)
                    {
                        if (SwitchStatus.NC == switchItemWithDeviceNumber.SwitchItem.SwitchStatus)
                        {
                            if (KnockTimes > knockingcount)
                            {
                                knockingcount++;
                                CallOnNotification($"count: {knockingcount}");
                            }
                            else
                            {
                                OnGameOver();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    knockingcount = 0;
                    CallOnNotification($"timeout, count: {knockingcount}");
                }
                //await Task.Delay(1000);
            }
        }
    }
}