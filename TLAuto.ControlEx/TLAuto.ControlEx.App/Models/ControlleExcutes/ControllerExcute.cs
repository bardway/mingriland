// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Board;
using TLAuto.ControlEx.App.Models.ControlleExcutes.DMX;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Notification;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes
{
    [XmlInclude(typeof(BoardControllerExcute))]
    [XmlInclude(typeof(DelayControllerExcute))]
    [XmlInclude(typeof(MusicControllerExcute))]
    [XmlInclude(typeof(NotificationControllerExcute))]
    [XmlInclude(typeof(ProjectorControllerExcute))]
    [XmlInclude(typeof(DMXControllerExcute))]
    public abstract class ControllerExcute : ObservableObject
    {
        private string _description;
        private bool _isChecked;

        [XmlIgnore]
        public bool IsChecked
        {
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
            get => _isChecked;
        }

        public string Description
        {
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
            get => _description.IsNullOrEmpty() ? GetDefaultDescription() : _description;
        }

        protected virtual string GetDefaultDescription()
        {
            return "默认执行器描述";
        }

        public virtual Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            return Task.Factory.StartNew(() => true);
        }

        public virtual void StopExcute(Action<string> writeLogMsgAction) { }

        public virtual void BreakExcute(Action<string> writeLogMsgAction) { }
    }
}