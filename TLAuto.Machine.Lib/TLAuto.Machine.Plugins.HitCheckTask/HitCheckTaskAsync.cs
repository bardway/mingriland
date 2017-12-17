// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;

using TLAuto.Base.Async;
using TLAuto.Log;
#endregion

namespace TLAuto.Machine.Plugins.HitCheckTask
{
    public sealed class HitCheckTaskAsync : AsyncTaskBase<List<SwitchItemWithDeviceNumber>>
    {
        private readonly HitCheckTask _hitCheckTask;
        private readonly List<HitCheckTask> _hitCheckTasks;
        private readonly LogWraper _log = new LogWraper(nameof(HitCheckTaskAsync));
        private bool _isPost;

        public HitCheckTaskAsync(HitCheckTask hitCheckTask, int timeout = 4000) : base(null, timeout)
        {
            _hitCheckTask = hitCheckTask;
            _hitCheckTask.NotifySwitchItem += HitCheckTask_NotifySwitchItem;
        }

        public HitCheckTaskAsync(List<HitCheckTask> hitCheckTasks, int timeout = 4000) : base(null, timeout)
        {
            _hitCheckTasks = hitCheckTasks;
            foreach (var checkTask in _hitCheckTasks)
            {
                checkTask.NotifySwitchItem += HitCheckTask_NotifySwitchItem;
            }
        }

        private void HitCheckTask_NotifySwitchItem(object sender, EventArgs e)
        {
            if (_isPost)
            {
                return;
            }
            var messages = HitTaskCache.GetMessages();

            //foreach (var message in messages)
            //{
            //    Debug.WriteLine($"  stamp: {message.Stamp.ToString().Remove(8)} switchnumber: {message.SwitchItem.SwitchNumber} devicenumber: {message.DeviceNumber} switchstatus: {message.SwitchItem.SwitchStatus} queuecount: {HitTaskCache.CountQueue()}");
            //}

            _isPost = true;
            PostResult(messages);
        }

        protected override void Invoke()
        {
            if (null != _hitCheckTasks)
            {
                foreach (var hitCheckTask in _hitCheckTasks)
                {
                    if (!hitCheckTask.IsReg)
                    {
                        if (!hitCheckTask.Reg())
                        {
                            PostResult(null);
                        }
                    }
                }
            }
            else
            {
                if (!_hitCheckTask.IsReg)
                {
                    if (!_hitCheckTask.Reg())
                    {
                        PostResult(null);
                    }
                }
            }
        }

        protected override void Exception(Exception ex)
        {
            _log.Critical(ex.Message, ex);
        }

        protected override void Dispose(bool disposing)
        {
            if (null != _hitCheckTasks)
            {
                foreach (var hitCheckTask in _hitCheckTasks)
                {
                    hitCheckTask.NotifySwitchItem -= HitCheckTask_NotifySwitchItem;
                }
            }
            else
            {
                _hitCheckTask.NotifySwitchItem -= HitCheckTask_NotifySwitchItem;
            }
        }
    }
}