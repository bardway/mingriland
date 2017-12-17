// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Controls.NavFrame
{
    public class NavStatusChangedEventArgs : EventArgs
    {
        public NavStatusChangedEventArgs(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }

        public bool IsCompleted { get; }
    }
}