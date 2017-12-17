// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.CheckPressButtons.Models
{
    public class CheckButtonItem
    {
        public CheckButtonItem(MachineButtonItem buttonItem)
        {
            ButtonItem = buttonItem;
        }

        public MachineButtonItem ButtonItem { get; }

        public bool IsPress { set; get; }

        public string IsPressStatus => IsPress ? "吸合" : "非吸合";
    }
}