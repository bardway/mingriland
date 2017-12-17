// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Notification;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditAppInputBoardParamDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditAppBoardParamDialog : Window
    {
        private readonly ObservableCollection<BoardParamInfo> _boardParamInfos;

        public EditAppBoardParamDialog(BoardType boardType, ObservableCollection<BoardParamInfo> boardParamInfos)
        {
            InitializeComponent();
            BoardType = boardType;
            _boardParamInfos = boardParamInfos;
            LstBoardPramInfos.ItemsSource = _boardParamInfos;
            foreach (var boardParamInfo in _boardParamInfos)
            {
                boardParamInfo.BoardType = boardType;
            }
        }

        public BoardType BoardType { get; }

        private void AddBoardItem_Click(object sender, RoutedEventArgs e)
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
                    if (ProjectHelper.FindServiceAddressMark(ProjectHelper.Project, BoardType, deviceNumber, out serviceAddressMark))
                    {
                        _boardParamInfos.Add(new BoardParamInfo
                                             {
                                                 DeviceNumber = deviceNumber,
                                                 Number = number,
                                                 BoardType = BoardType,
                                                 ServiceAddressMark = serviceAddressMark,
                                                 PortName = portName
                                             });
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
        }

        private void RemoveBoardItem_Click(object sender, RoutedEventArgs e)
        {
            var removeInfo = _boardParamInfos.Where(s => s.IsChecked).ToList();
            foreach (var boardParamInfo in removeInfo)
            {
                _boardParamInfos.Remove(boardParamInfo);
            }
        }

        private void UpBoardItem_Click(object sender, RoutedEventArgs e)
        {
            var boardItemInfo = _boardParamInfos.FirstOrDefault(s => s.IsChecked);
            if (boardItemInfo != null)
            {
                _boardParamInfos.Up(boardItemInfo);
            }
        }

        private void DownBoardItem_Click(object sender, RoutedEventArgs e)
        {
            var boardItemInfo = _boardParamInfos.FirstOrDefault(s => s.IsChecked);
            if (boardItemInfo != null)
            {
                _boardParamInfos.Down(boardItemInfo);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}