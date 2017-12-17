// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.Maid
{
    public sealed class LineButton
    {
        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public LineButton(MachineButtonItem machineButtonItem, string voice = null, int duration = 0)
        {
            Voice = voice;
            Duration = duration;
            MachineButtonItem = machineButtonItem;
        }

        public string Voice { get; set; }

        public int Duration { get; set; }

        public MachineButtonItem MachineButtonItem { get; }
    }
}