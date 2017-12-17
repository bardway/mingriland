// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
#endregion

namespace TLAuto.ControlEx.App.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var a = Convert.ToByte(ConfigurationManager.AppSettings["BgColorA"]);
            var b = Convert.ToByte(ConfigurationManager.AppSettings["BgColorB"]);
            var c = Convert.ToByte(ConfigurationManager.AppSettings["BgColorC"]);
            Tc.Background = new SolidColorBrush(Color.FromRgb(a, b, c));
        }
    }
}