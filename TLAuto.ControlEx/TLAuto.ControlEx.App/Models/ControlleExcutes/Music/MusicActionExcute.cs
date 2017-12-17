// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Music
{
    [XmlInclude(typeof(PlayMusicActionExcute))]
    [XmlInclude(typeof(PauseMusicActionExcute))]
    [XmlInclude(typeof(AdjustVolumeMusicActionExcute))]
    [XmlInclude(typeof(TextToSpeakActionExcute))]
    public abstract class MusicActionExcute : ObservableObject
    {
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

        public abstract Task<bool> Excute(Action<string> writeLogMsgAction);
    }
}