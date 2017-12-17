// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Notification
{
    public class BoardParamInfo : ObservableObject
    {
        private int _deviceNumber;

        private bool _isChecked;

        private int _number;

        private string _portName;

        private string _serviceAddressMark;

        public BoardParamInfo()
        {
            InitEditCommand();
        }

        [XmlIgnore]
        public BoardType BoardType { set; get; }

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

        public string ServiceAddressMark
        {
            set
            {
                _serviceAddressMark = value;
                RaisePropertyChanged();
            }
            get => _serviceAddressMark;
        }

        public string PortName
        {
            set
            {
                _portName = value;
                RaisePropertyChanged();
            }
            get => _portName;
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

        #region Event MvvmBindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var sbi = new SelectBoardItemDialog(BoardType == BoardType.InputA ? ProjectHelper.Project.ItemXmlInfo.InputBoardGroup : ProjectHelper.Project.ItemXmlInfo.OutputBoardGroup, false);
                                               if (sbi.ShowDialog() == true)
                                               {
                                                   var boardItem = sbi.SelectedBoardItem;
                                                   var deviceNumber = sbi.SelectedBoard.DeviceNumber;
                                                   var number = boardItem.Number;
                                                   string portName;
                                                   if (ProjectHelper.FindPortName(ProjectHelper.Project, BoardType, deviceNumber, out portName))
                                                   {
                                                       string serviceAddressMark;
                                                       if (ProjectHelper.FindServiceAddressMark(ProjectHelper.Project,
                                                                                                BoardType,
                                                                                                deviceNumber,
                                                                                                out serviceAddressMark))
                                                       {
                                                           DeviceNumber = deviceNumber;
                                                           Number = number;
                                                           ServiceAddressMark = serviceAddressMark;
                                                           PortName = portName;
                                                       }
                                                       else
                                                       {
                                                           MessageBox.Show("请先设置工业版通讯地址。");
                                                       }
                                                   }
                                                   else
                                                   {
                                                       MessageBox.Show("请先设置工业版串口号。");
                                                   }
                                               }
                                           });
        }

        [XmlIgnore]
        public RelayCommand EditCommand { private set; get; }
        #endregion
    }
}