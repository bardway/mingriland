// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Machine.Plugins.Maid
{
    public sealed class Step
    {
        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public Step(int stepNo, string voice, int duration, List<LineButton> buttonItems, string said = null)
        {
            StepNo = stepNo;
            Duration = duration;
            Voice = voice;
            ButtonItems = buttonItems;
            Said = said;
        }

        public int StepNo { get; set; }

        public int Duration { get; set; }

        public string Voice { get; set; }

        public List<LineButton> ButtonItems { get; set; }

        public string Said { get; set; }
    }
}