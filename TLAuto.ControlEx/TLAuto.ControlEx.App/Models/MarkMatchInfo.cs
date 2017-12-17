// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    [XmlInclude(typeof(ProjectorMarkMatchInfo))]
    [XmlInclude(typeof(MusicMarkMatchInfo))]
    public class MarkMatchInfo : ObservableObject
    {
        private bool _isChecked;
        private string _mark;

        private string _serviceAddressMark;

        public string Mark
        {
            set
            {
                _mark = value;
                RaisePropertyChanged();
            }
            get => _mark;
        }

        public string ServiceAddressMark
        {
            set
            {
                _serviceAddressMark = value;
                RaisePropertyChanged();
            }
            get => _serviceAddressMark;
        }

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
    }
}