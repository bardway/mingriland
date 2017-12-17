// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes
{
    [Description("延时执行器")]
    public class DelayControllerExcute : ControllerExcute
    {
        private string _hours = "0";
        private volatile bool _isBreak;
        private volatile bool _isStop;

        private string _minutes = "0";

        private string _seconds = "0";

        public string Hours
        {
            set
            {
                _hours = value;
                RaisePropertyChanged();
            }
            get => _hours;
        }

        public string Minutes
        {
            set
            {
                _minutes = value;
                RaisePropertyChanged();
            }
            get => _minutes;
        }

        public string Seconds
        {
            set
            {
                _seconds = value;
                RaisePropertyChanged();
            }
            get => _seconds;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            _isStop = false;
            _isBreak = false;
            var time = GetAllTime();
            writeLogMsgAction("延时：" + time.TotalSeconds + " 秒");
            var seconds = (int)time.TotalSeconds;
            await Task.Factory.StartNew(() =>
                                        {
                                            while ((seconds != 0) && !_isStop && !_isBreak)
                                            {
                                                Thread.Sleep(1000);
                                                seconds--;
                                            }
                                        });
            if (_isStop)
            {
                return false;
            }
            return true;
        }

        private TimeSpan GetAllTime()
        {
            return new TimeSpan(Hours.ToInt32(), Minutes.ToInt32(), Seconds.ToInt32());
        }

        public override void StopExcute(Action<string> writeLogMsgAction)
        {
            _isStop = true;
        }

        public override void BreakExcute(Action<string> writeLogMsgAction)
        {
            _isBreak = true;
        }
    }
}