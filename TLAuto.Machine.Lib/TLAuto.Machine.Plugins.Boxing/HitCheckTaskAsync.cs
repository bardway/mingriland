// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;

using TLAuto.Base.Async;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Log;
using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.Boxing
{
    public class HitCheckTaskAsync : AsyncTaskBase<bool>
    {
        private readonly List<MachineButtonItem> _buttonItems;
        private readonly HitCheckTask _hitCheckTask;
        private readonly object _lock = new object();
        private readonly LogWraper _log = new LogWraper(nameof(HitCheckTaskAsync));
        private readonly MachineButtonItem _needButtonItem;
        private bool _isPost;

        public HitCheckTaskAsync(HitCheckTask hitCheckTask, List<MachineButtonItem> buttonItems, MachineButtonItem needButtonItem, int timeOutMs = 4000) : base(null, timeOutMs)
        {
            _hitCheckTask = hitCheckTask;
            _buttonItems = buttonItems;
            _needButtonItem = needButtonItem;
            _hitCheckTask.NotifySwitchItem += HitCheckTask_NotifySwitchItem;
        }

        private void HitCheckTask_NotifySwitchItem(object sender, SwitchItem e)
        {
            if (_isPost)
            {
                return;
            }
            var buttonItem = _buttonItems.FirstOrDefault(s => s.Number == e.SwitchNumber);
            if ((buttonItem != null) && (e.SwitchNumber == _needButtonItem.Number) &&
                (e.SwitchStatus == SwitchStatus.NC))
            {
                _isPost = true;
                PostResult(true);
                return;
            }
            if (buttonItem == null)
            {
                return;
            }
            PostResult(false);
        }

        protected override void Invoke()
        {
            if (!_hitCheckTask.IsReg)
            {
                if (!_hitCheckTask.Reg())
                {
                    PostResult(false);
                }
            }
        }

        protected override void Exception(Exception ex)
        {
            _log.Critical(ex.Message, ex);
        }

        protected override void Dispose(bool disposing)
        {
            _hitCheckTask.NotifySwitchItem -= HitCheckTask_NotifySwitchItem;
        }
    }
}