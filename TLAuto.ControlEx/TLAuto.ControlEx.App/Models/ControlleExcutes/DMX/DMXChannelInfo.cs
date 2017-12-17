// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.DMX
{
    public class DMXChannelInfo : ObservableObject
    {
        private int _channelNumber;

        private int _channelValue;

        private bool _isChecked;

        public int ChannelNumber
        {
            set
            {
                _channelNumber = value;
                RaisePropertyChanged();
            }
            get => _channelNumber;
        }

        public int ChannelValue
        {
            set
            {
                _channelValue = value;
                RaisePropertyChanged();
            }
            get => _channelValue;
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