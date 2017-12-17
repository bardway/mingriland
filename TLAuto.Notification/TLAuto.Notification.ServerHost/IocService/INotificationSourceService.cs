// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Notification.Contracts;
using TLAuto.Notification.ServerHost.ViewModels;
#endregion

namespace TLAuto.Notification.ServerHost.IocService
{
    public interface INotificationSourceService : ITLNotification
    {
        void SetNotificationUI(NotificationViewModel notificationVm);
    }
}