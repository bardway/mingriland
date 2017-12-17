// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Board
{
    [XmlInclude(typeof(SwitchBoardControllerExcute))]
    [XmlInclude(typeof(RelayBoardControllerExcute))]
    public abstract class BoardControllerExcute : ControllerExcute
    {
        public BoardControllerExcute()
        {
            InitAddBoardItemExcuteCommand();
            InitRemoveBoardItemExcuteCommand();
            InitUpBoardItemExcuteCommand();
            InitDownBoardItemExcuteCommand();
        }

        public ObservableCollection<BoardItemInfo> BoardItemInfos { get; } = new ObservableCollection<BoardItemInfo>();

        [XmlIgnore]
        public BoardType BoardType { protected set; get; }

        #region Event MvvmBindings
        private void InitAddBoardItemExcuteCommand()
        {
            AddBoardItemExcuteCommand = new RelayCommand(() =>
                                                         {
                                                             var sbi = new SelectBoardItemDialog(BoardType == BoardType.InputA ? ProjectHelper.Project.ItemXmlInfo.InputBoardGroup : ProjectHelper.Project.ItemXmlInfo.OutputBoardGroup, true);
                                                             if (sbi.ShowDialog() == true)
                                                             {
                                                                 var board = sbi.SelectedBoard;
                                                                 var boardItem = sbi.SelectedBoardItem;
                                                                 BoardItemInfo boardItemInfo;
                                                                 switch (board.BoardType)
                                                                 {
                                                                     case BoardType.InputA:
                                                                         boardItemInfo = new SwitchBoardItemInfo();
                                                                         break;
                                                                     case BoardType.OutputA:
                                                                         boardItemInfo = new RelayBoardItemInfo();
                                                                         break;
                                                                     default:
                                                                         throw new ArgumentOutOfRangeException();
                                                                 }
                                                                 boardItemInfo.DeviceNumber = board.DeviceNumber;
                                                                 boardItemInfo.Number = boardItem.Number;
                                                                 boardItemInfo.IsNo = sbi.IsNo;
                                                                 BoardItemInfos.Add(boardItemInfo);
                                                             }
                                                         });
        }

        [XmlIgnore]
        public RelayCommand AddBoardItemExcuteCommand { private set; get; }

        private void InitRemoveBoardItemExcuteCommand()
        {
            RemoveBoardItemExcuteCommand = new RelayCommand(() =>
                                                            {
                                                                var removeInfo = BoardItemInfos.Where(s => s.IsChecked).ToList();
                                                                foreach (var switchInfo in removeInfo)
                                                                {
                                                                    BoardItemInfos.Remove(switchInfo);
                                                                }
                                                            });
        }

        [XmlIgnore]
        public RelayCommand RemoveBoardItemExcuteCommand { private set; get; }

        private void InitUpBoardItemExcuteCommand()
        {
            UpBoardItemExcuteCommand = new RelayCommand(() =>
                                                        {
                                                            var boardItemInfo = BoardItemInfos.FirstOrDefault(s => s.IsChecked);
                                                            if (boardItemInfo != null)
                                                            {
                                                                BoardItemInfos.Up(boardItemInfo);
                                                            }
                                                        });
        }

        [XmlIgnore]
        public RelayCommand UpBoardItemExcuteCommand { private set; get; }

        private void InitDownBoardItemExcuteCommand()
        {
            DownBoardItemExcuteCommand = new RelayCommand(() =>
                                                          {
                                                              var boardItemInfo = BoardItemInfos.FirstOrDefault(s => s.IsChecked);
                                                              if (boardItemInfo != null)
                                                              {
                                                                  BoardItemInfos.Down(boardItemInfo);
                                                              }
                                                          });
        }

        [XmlIgnore]
        public RelayCommand DownBoardItemExcuteCommand { private set; get; }
        #endregion
    }
}