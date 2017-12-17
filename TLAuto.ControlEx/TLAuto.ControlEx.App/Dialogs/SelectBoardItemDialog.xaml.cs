// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Dialogs.Models;
using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// SelectBoardItemDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectBoardItemDialog : Window
    {
        private readonly List<DialogBoard> _boards = new List<DialogBoard>();

        public SelectBoardItemDialog(BoardGroupXmlInfo boardGroup, bool isVisibleStatusBar)
        {
            InitializeComponent();
            InitBoards(boardGroup);
            CboDevices.ItemsSource = _boards;
            if (!isVisibleStatusBar)
            {
                TblStatusTitle.Visibility = Visibility.Collapsed;
                CboStatus.Visibility = Visibility.Collapsed;
            }
            IsVisibleStatusBar = isVisibleStatusBar;
        }

        public bool IsVisibleStatusBar { get; }

        public DialogBoard SelectedBoard => (DialogBoard)CboDevices.SelectedItem;

        public DialogBoardItem SelectedBoardItem => (DialogBoardItem)BoardItemsControl.SelectedItem;

        public bool IsNo
        {
            get
            {
                if (!IsVisibleStatusBar)
                {
                    throw new ArgumentException("IsVisibleStatusBar is false.");
                }
                return CboStatus.SelectedValue.ToBoolean();
            }
        }

        private void InitBoards(BoardGroupXmlInfo boardGroup)
        {
            foreach (var boardInfo in boardGroup.BoardInfos)
            {
                var dialogBoard = new DialogBoard(boardInfo.DeviceNumber, boardInfo.BoardName, boardGroup.BoardType);
                foreach (var boardItem in boardInfo.BoardItemInfos)
                {
                    dialogBoard.BoardItems.Add(new DialogBoardItem(boardItem.Number, boardItem.ToolTip));
                }
                _boards.Add(dialogBoard);
            }
        }

        private void CboDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var board = e.AddedItems[0] as DialogBoard;
            if (board != null)
            {
                BoardItemsControl.ItemsSource = board.BoardItems;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (BoardItemsControl.SelectedItem != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}