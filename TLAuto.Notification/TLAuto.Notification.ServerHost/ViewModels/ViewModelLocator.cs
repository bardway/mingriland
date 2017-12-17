// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;

using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;

using TLAuto.Notification.ServerHost.IocService;
#endregion

namespace TLAuto.Notification.ServerHost.ViewModels
{
    [EditorBrowsable(EditorBrowsableState.Always)]
    public sealed class ViewModelLocator
    {
        public static readonly ViewModelLocator Instance = new ViewModelLocator();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<INotificationSourceService>())
            {
                SimpleIoc.Default.Register<INotificationSourceService, WcfNotificationSourceService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public INotificationSourceService NotificationSourceService => SimpleIoc.Default.GetInstance<INotificationSourceService>();

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}