// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.LocalVideo.App.Common;
#endregion

namespace TLAuto.LocalVideo.App.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!StartupArgsHelper.VideoFilePath.IsNullOrEmpty())
            {
                Vc.Source = new Uri(StartupArgsHelper.VideoFilePath, UriKind.Absolute);
                Vc.Volume = 1;
                Vc.IsRepeat = StartupArgsHelper.IsRepeat;
            }
        }
    }
}