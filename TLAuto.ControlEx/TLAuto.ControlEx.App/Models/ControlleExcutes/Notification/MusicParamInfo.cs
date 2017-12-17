// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Notification
{
    public class MusicParamInfo : ObservableObject
    {
        private bool _isChecked;

        public string ServiceAddressMark { set; get; }

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