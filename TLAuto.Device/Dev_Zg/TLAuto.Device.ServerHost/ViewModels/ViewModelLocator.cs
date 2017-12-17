// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;

using TLAuto.Device.ServerHost.IocService;
#endregion

namespace TLAuto.Device.ServerHost.ViewModels
{
    public sealed class ViewModelLocator
    {
        public static readonly ViewModelLocator Instance = new ViewModelLocator();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<ITLAutoDeviceIocService>())
            {
                SimpleIoc.Default.Register<ITLAutoDeviceIocService, WcfTLAutoDeviceIocService>(true);
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        public ITLAutoDeviceIocService TLAutoDeviceIocService => SimpleIoc.Default.GetInstance<ITLAutoDeviceIocService>();

        public ITLAutoDevicePushIocService TLAutoDevicePushIocService => (ITLAutoDevicePushIocService)TLAutoDeviceIocService;
    }
}