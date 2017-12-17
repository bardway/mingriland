// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight.Messaging;
#endregion

namespace TLAuto.Device.Controls.RichTextBoxEx
{
    public class StatusNotificationMessage : NotificationMessage
    {
        public StatusNotificationMessage(string notification, StatusNotificationType statusType)
            : this(null, null, notification, statusType) { }

        public StatusNotificationMessage(object sender, string notification, StatusNotificationType statusType)
            : this(sender, null, notification, statusType) { }

        public StatusNotificationMessage(object sender, object target, string notification, StatusNotificationType statusType)
            : base(sender, target, notification)
        {
            StatusType = statusType;
        }

        public StatusNotificationType StatusType { get; }
    }
}