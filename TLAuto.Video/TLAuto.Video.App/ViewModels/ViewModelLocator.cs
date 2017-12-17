// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;

using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;
#endregion

namespace TLAuto.Video.App.ViewModels
{
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class ViewModelLocator
    {
        public static readonly ViewModelLocator Instance = new ViewModelLocator();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}