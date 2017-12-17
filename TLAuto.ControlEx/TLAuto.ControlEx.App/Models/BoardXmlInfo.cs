// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class BoardXmlInfo : ObservableObject
    {
        private string _boardName;

        private int _deviceNumber = 1;

        private bool _isChecked;

        private string _serviceAddressMark;

        private string _signName;

        public BoardXmlInfo()
        {
            InitEditCommand();
            BoardItemInfos.CollectionChanged += BoardItemInfos_CollectionChanged;
        }

        public string BoardName
        {
            set
            {
                _boardName = value;
                RaisePropertyChanged();
            }
            get => _boardName;
        }

        public int DeviceNumber
        {
            set
            {
                _deviceNumber = value;
                RaisePropertyChanged();
            }
            get => _deviceNumber;
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

        public string SignName
        {
            set
            {
                _signName = value;
                RaisePropertyChanged();
            }
            get => _signName;
        }

        public ObservableCollection<BoardItemXmlInfo> BoardItemInfos { get; } = new ObservableCollection<BoardItemXmlInfo>();

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

        private void BoardItemInfos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BoardItemXmlInfo newItem in e.NewItems)
                {
                    newItem.Parent = this;
                }
            }
        }

        #region Event MvvmBindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var ebd = new EditBoardDialog(BoardName, DeviceNumber, ServiceAddressMark, SignName, ProjectHelper.Project.ItemXmlInfo.BoardServiceAddressInfos);
                                               if (ebd.ShowDialog() == true)
                                               {
                                                   BoardName = ebd.BoardName;
                                                   DeviceNumber = ebd.DeviceNumber;
                                                   ServiceAddressMark = ebd.SelectedServiceAddressMark;
                                                   SignName = ebd.SignName;
                                               }
                                           });
        }

        [XmlIgnore]
        public RelayCommand EditCommand { private set; get; }
        #endregion
    }
}