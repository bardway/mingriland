// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            OnBreakThird();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            OnBreakUnThird();
        }

        public event EventHandler BreakThird;

        protected virtual void OnBreakThird()
        {
            BreakThird?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler BreakUnThird;

        protected virtual void OnBreakUnThird()
        {
            BreakUnThird?.Invoke(this, EventArgs.Empty);
        }
    }
}