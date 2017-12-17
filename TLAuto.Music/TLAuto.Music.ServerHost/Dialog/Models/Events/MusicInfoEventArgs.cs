// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Music.ServerHost.Dialog.Models.Events
{
    public class MusicInfoEventArgs : EventArgs
    {
        public MusicInfoEventArgs(string mark, string filePath = null, double volume = 0.5, bool isRepeat = false)
        {
            Mark = mark;
            FilePath = filePath;
            Volume = volume;
            IsRepeat = isRepeat;
        }

        public string Mark { get; }

        public string FilePath { get; }

        public double Volume { get; }

        public bool IsRepeat { get; }
    }
}