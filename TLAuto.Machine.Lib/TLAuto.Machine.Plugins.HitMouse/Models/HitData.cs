// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models
{
    public class HitData
    {
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        private int _hitCount = 0;

        public int HitCount { set; get; }

        public int ChargeHitCount { set; get; }

        public int CheckTime { set; get; }

        public Func<int, Task<bool>> Func { private set; get; }

        public async Task<bool> CheckHit(List<MachineButtonItem> checkMachineItems, List<MachineRelayItem> checkRelayItems, HitMouseMachinePluginsProvider provider, bool isAdjoin = false)
        {
            var checkLightButtonItemsDic = new Dictionary<int, MachineButtonItem>();
            var checkLightItems = new Dictionary<int, MachineRelayItem>();

            for (var i = 0; i < HitCount; i++)
            {
                ReRandom:
                var tick = DateTime.Now.Ticks;
                var random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                var index = random.Next(0, checkMachineItems.Count);
                if (isAdjoin)
                {
                    if (checkLightButtonItemsDic.Keys.Any(s => (s + 4 == index) || (s - 4 == index)))
                    {
                        goto ReRandom;
                    }
                }
                if (!checkLightButtonItemsDic.ContainsKey(index))
                {
                    checkLightButtonItemsDic.Add(index, checkMachineItems[index]);
                    checkLightItems.Add(index, checkRelayItems[index]);
                }
                else
                {
                    goto ReRandom;
                }
            }
            var checkLightButtonItems = checkLightButtonItemsDic.Values.ToList();
            foreach (var machineRelayItem in checkLightItems.Values)
            {
                await machineRelayItem.Control(true);
            }
            _manualResetEvent.Reset();
            var checkNumbers = new List<int>();

            Func = async switchNumber =>
                   {
                       if (checkNumbers.Contains(switchNumber) || (checkNumbers.Count == checkLightButtonItems.Count))
                       {
                           return false;
                       }
                       var relayIndex =
                           checkLightButtonItems.IndexOf(checkLightButtonItems.FirstOrDefault(s => s.Number == switchNumber));
                       if (relayIndex < 0)
                       {
                           return false;
                       }
                       checkNumbers.Add(switchNumber);
                       await checkLightItems.Values.ToList()[relayIndex].Control(false);
                       await PlayHitMusic(provider);
                       if (checkNumbers.Count == checkLightButtonItems.Count)
                       {
                           _manualResetEvent.Set();
                       }
                       return true;
                   };
            var result = _manualResetEvent.WaitOne(CheckTime * 1000);
            foreach (var machineRelayItem in checkLightItems.Values)
            {
                await machineRelayItem.Control(false);
            }
            return result;
        }

        private async Task PlayHitMusic(HitMouseMachinePluginsProvider provider)
        {
            if (provider._isChanged)
            {
                await provider.PlayMusic0(HitMouseMachinePluginsProvider.SignKey, "hit1.wav", "hit1");
                provider._isChanged = !provider._isChanged;
            }
            else
            {
                await provider.PlayMusic0(HitMouseMachinePluginsProvider.SignKey, "hit2.wav", "hit2");
                provider._isChanged = !provider._isChanged;
            }
        }
    }
}