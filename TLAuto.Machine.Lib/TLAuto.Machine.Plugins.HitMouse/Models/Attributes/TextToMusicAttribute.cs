// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models.Attributes
{
    public class TextToMusicAttribute : Attribute
    {
        public TextToMusicAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}