// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Music.ServerHost.Dialog.Models
{
    public class DialogMusicInfo
    {
        public DialogMusicInfo(string mark)
        {
            Mark = mark;
        }

        public string Mark { get; }

        public string FilePath { set; get; }

        public double Volume { set; get; }

        public bool IsRepeat { set; get; }
    }
}