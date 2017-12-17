// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class BoardGroupXmlInfo : ObservableObject
    {
        private bool _isExpanded = true;

        public BoardGroupXmlInfo()
        {
            InitAddBoardCommand();
            InitRemoveBoardCommand();
            InitUpBoardCommand();
            InitDownBoardCommand();
        }

        public BoardType BoardType { set; get; }

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                RaisePropertyChanged();
            }
            get => _isExpanded;
        }

        public ObservableCollection<BoardXmlInfo> BoardInfos { get; } = new ObservableCollection<BoardXmlInfo>();

        [XmlIgnore]
        public RelayCommand AddBoardCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand RemoveBoardCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand UpBoardCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand DownBoardCommand { private set; get; }

        private void InitAddBoardCommand()
        {
            AddBoardCommand = new RelayCommand(() =>
                                               {
                                                   var nbd = new NewBoardDialog(BoardType, string.Empty, ProjectHelper.Project.ItemXmlInfo.BoardServiceAddressInfos);
                                                   if (nbd.ShowDialog() == true)
                                                   {
                                                       var boardName = nbd.BoardName;
                                                       var deviceNumber = nbd.DeviceNumber;
                                                       var itemCount = nbd.ItemCount;
                                                       var boardInfo = new BoardXmlInfo
                                                                       {
                                                                           BoardName = boardName,
                                                                           DeviceNumber = deviceNumber,
                                                                           ServiceAddressMark = nbd.SelectedServiceAddressMark,
                                                                           SignName = nbd.SignName
                                                                       };
                                                       for (var i = 0; i < itemCount; i++)
                                                       {
                                                           boardInfo.BoardItemInfos.Add(new BoardItemXmlInfo
                                                                                        {
                                                                                            Number = i + 1
                                                                                        });
                                                       }
                                                       BoardInfos.Add(boardInfo);
                                                   }
                                               });
        }

        private void InitRemoveBoardCommand()
        {
            RemoveBoardCommand = new RelayCommand(() =>
                                                  {
                                                      var removeList = BoardInfos.Where(s => s.IsChecked).ToList();
                                                      foreach (var boardInfo in removeList)
                                                      {
                                                          BoardInfos.Remove(boardInfo);
                                                      }
                                                  });
        }

        private void InitUpBoardCommand()
        {
            UpBoardCommand = new RelayCommand(() =>
                                              {
                                                  var boardInfo = BoardInfos.FirstOrDefault(s => s.IsChecked);
                                                  if (boardInfo != null)
                                                  {
                                                      BoardInfos.Up(boardInfo);
                                                  }
                                              });
        }

        private void InitDownBoardCommand()
        {
            DownBoardCommand = new RelayCommand(() =>
                                                {
                                                    var boardInfo = BoardInfos.FirstOrDefault(s => s.IsChecked);
                                                    if (boardInfo != null)
                                                    {
                                                        BoardInfos.Down(boardInfo);
                                                    }
                                                });
        }
    }
}