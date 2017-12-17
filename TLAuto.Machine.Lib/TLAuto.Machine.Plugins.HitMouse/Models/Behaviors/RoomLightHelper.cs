// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;

using TLAuto.Base.Async;
using TLAuto.Base.Extensions;
using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models.Behaviors
{
    public static class RoomLightHelper
    {
        private static readonly Timer _timer = new Timer {AutoReset = false};
        private static readonly double _timeDelay1 = ConfigurationManager.AppSettings["HitRoomLightDelay1"].ToDouble();
        private static readonly double _timeDelay2 = ConfigurationManager.AppSettings["HitRoomLightDelay2"].ToDouble();
        private static readonly AsyncLock LockAsync = new AsyncLock();
        private static volatile bool _isStop;

        static RoomLightHelper()
        {
            _timer.Interval = _timeDelay1;
            _timer.Elapsed += Timer_Elapsed;
        }

        public static MachineRelayItem RoomRelayLight { set; get; }

        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (var releaser = await LockAsync.LockAsync())
            {
                if (_isStop)
                {
                    return;
                }
                await RoomRelayLight.Control(false);
                await Task.Delay(TimeSpan.FromMilliseconds(_timeDelay2));
                await RoomRelayLight.Control(true);
                _timer.Start();
            }
        }

        public static void Start()
        {
            _isStop = false;
            _timer.Start();
        }

        public static async Task Stop()
        {
            using (var releaser = await LockAsync.LockAsync())
            {
                _timer.Stop();
                _isStop = true;
            }
        }
    }
}