// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
#endregion

namespace TLAuto.Machine.App.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, s => { DispatcherHelper.CheckBeginInvokeOnUI(() => { McControl.LogToUi(s.Notification); }); });
        }
    }
}