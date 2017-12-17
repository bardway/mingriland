// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Board
{
    [XmlInclude(typeof(SwitchBoardItemInfo))]
    [XmlInclude(typeof(RelayBoardItemInfo))]
    public abstract class BoardItemInfo : ObservableObject
    {
        private int _deviceNumber = 1;

        private bool _isChecked;

        private bool _isNo;

        private int _number;

        public int DeviceNumber
        {
            set
            {
                _deviceNumber = value;
                RaisePropertyChanged();
            }
            get => _deviceNumber;
        }

        public int Number
        {
            set
            {
                _number = value;
                RaisePropertyChanged();
            }
            get => _number;
        }

        public bool IsNo
        {
            set
            {
                _isNo = value;
                RaisePropertyChanged();
            }
            get => _isNo;
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