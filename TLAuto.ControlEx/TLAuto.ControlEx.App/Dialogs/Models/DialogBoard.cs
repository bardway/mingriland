// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Dialogs.Models
{
    public class DialogBoard
    {
        public DialogBoard(int deviceNumber, string boardName, BoardType boardType)
        {
            DeviceNumber = deviceNumber;
            BoardName = boardName;
            BoardType = boardType;
        }

        public string BoardName { get; }

        public int DeviceNumber { get; }

        public BoardType BoardType { get; }

        public List<DialogBoardItem> BoardItems { get; } = new List<DialogBoardItem>();
    }
}