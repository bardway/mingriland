// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.Candle.Models
{
    public class CheckButtonItem
    {
        public CheckButtonItem(MachineButtonItem buttonItem)
        {
            ButtonItem = buttonItem;
        }

        public MachineButtonItem ButtonItem { get; }

        public bool IsPress { set; get; }
    }
}