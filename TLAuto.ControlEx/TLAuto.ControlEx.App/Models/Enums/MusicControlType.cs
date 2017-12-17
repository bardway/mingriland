// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.ControlEx.App.Models.Enums
{
    public enum MusicControlType
    {
        [Description("播放音乐")]
        Play,

        [Description("暂停音乐")]
        Pause,

        [Description("调整音乐大小")]
        AdjustVolume,

        [Description("文本转语音")]
        TextToSpeek
    }
}