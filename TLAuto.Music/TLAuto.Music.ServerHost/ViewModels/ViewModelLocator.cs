// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;

using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;

using TLAuto.Music.ServerHost.IocService;
#endregion

namespace TLAuto.Music.ServerHost.ViewModels
{
    [EditorBrowsable(EditorBrowsableState.Always)]
    public sealed class ViewModelLocator
    {
        public static readonly ViewModelLocator Instance = new ViewModelLocator();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<IMusicSourceService>())
            {
                SimpleIoc.Default.Register<IMusicSourceService, WcfMusicSourceService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public IMusicSourceService MusicSourceService => SimpleIoc.Default.GetInstance<IMusicSourceService>();

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}