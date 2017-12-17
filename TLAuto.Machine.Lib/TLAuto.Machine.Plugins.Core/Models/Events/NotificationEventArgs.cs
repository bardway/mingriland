// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Machine.Plugins.Core.Models.Events
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(string notification)
        {
            Notification = notification;
        }

        public string Notification { get; }
    }
}