// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Board
{
    public class SwitchBoardItemInfo : BoardItemInfo
    {
        public SwitchBoardItemInfo()
        {
            InitEditCommand();
        }

        #region Event MvvmBindings
        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var sbi = new SelectBoardItemDialog(ProjectHelper.Project.ItemXmlInfo.InputBoardGroup, true);
                                               if (sbi.ShowDialog() == true)
                                               {
                                                   var boardItem = sbi.SelectedBoardItem;
                                                   DeviceNumber = sbi.SelectedBoard.DeviceNumber;
                                                   Number = boardItem.Number;
                                                   IsNo = sbi.IsNo;
                                               }
                                           });
        }

        [XmlIgnore]
        public RelayCommand EditCommand { private set; get; }
        #endregion
    }
}