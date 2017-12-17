// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Reflection;
using System.Windows;
#endregion

namespace TLAuto.Device.ServerHost.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Title += $" {Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}