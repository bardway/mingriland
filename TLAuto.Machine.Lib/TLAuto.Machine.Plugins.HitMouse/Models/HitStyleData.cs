// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models
{
    public class HitStyleData
    {
        public HitStyleData(int questionLightButtonIndex, IEnumerable<int> lightIndexs)
        {
            QuestionLightButtonIndex = questionLightButtonIndex;
            LightIndexs = lightIndexs.ToList();
        }

        public int QuestionLightButtonIndex { get; }

        public List<int> LightIndexs { get; }
    }
}