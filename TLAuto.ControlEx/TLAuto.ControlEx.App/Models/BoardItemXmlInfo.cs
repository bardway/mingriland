// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Common;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class BoardItemXmlInfo : ObservableObject
    {
        private bool _isNo;

        private bool _isOpenToolTipPopup;

        private int _number;

        private string _toolTip;

        public BoardItemXmlInfo()
        {
            InitBoardItemClickCommand();
            InitToolTipOpenedCommand();
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

        [XmlIgnore]
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
        public BoardXmlInfo Parent { set; get; }

        public string ToolTip
        {
            set
            {
                _toolTip = value;
                RaisePropertyChanged();
            }
            get => _toolTip;
        }

        [XmlIgnore]
        public bool IsOpenToolTipPopup
        {
            set
            {
                _isOpenToolTipPopup = value;
                RaisePropertyChanged();
            }
            get => _isOpenToolTipPopup;
        }

        [XmlIgnore]
        public ObservableCollection<string> Descriptions { get; } = new ObservableCollection<string>();

        #region Event MvvmBindings
        private void InitBoardItemClickCommand()
        {
            BoardItemClickCommand = new RelayCommand(() => { IsOpenToolTipPopup = !IsOpenToolTipPopup; });
        }

        [XmlIgnore]
        public RelayCommand BoardItemClickCommand { private set; get; }

        private void InitToolTipOpenedCommand()
        {
            ToolTipOpenedCommand = new RelayCommand(() =>
                                                    {
                                                        Descriptions.Clear();
                                                        var descriptions = ProjectHelper.GetBoardExcuteDescriptioins(Parent.DeviceNumber, Number, ProjectHelper.Project);
                                                        foreach (var description in descriptions)
                                                        {
                                                            Descriptions.Add(description);
                                                        }
                                                    });
        }

        [XmlIgnore]
        public RelayCommand ToolTipOpenedCommand { private set; get; }
        #endregion
    }
}