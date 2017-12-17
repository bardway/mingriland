// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.ControlEx.App.Dialogs.Models
{
    public class DialogBoardItem
    {
        public DialogBoardItem(int number, string toolTip)
        {
            Number = number;
            ToolTip = toolTip;
        }

        public string ToolTip { get; }

        public int Number { get; }
    }
}