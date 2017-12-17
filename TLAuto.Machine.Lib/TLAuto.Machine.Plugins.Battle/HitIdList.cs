// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Machine.Plugins.Battle
{
    public class HitIdList : List<int>
    {
        public static HitIdList operator +(HitIdList l1, HitIdList l2)
        {
            var newlist = new HitIdList();
            newlist.AddRange(l1);
            newlist.AddRange(l2);
            return newlist;
        }
    }
}