// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;

using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;

using TLAuto.Machine.App.IocService;
#endregion

namespace TLAuto.Machine.App.ViewModels
{
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class ViewModelLocator
    {
        public static readonly ViewModelLocator Instance = new ViewModelLocator();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<IAppParamSourceService>())
            {
                SimpleIoc.Default.Register<IAppParamSourceService, AppParamSourceService>();
            }
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        public IAppParamSourceService AppParamSourceService => SimpleIoc.Default.GetInstance<IAppParamSourceService>();
    }
}