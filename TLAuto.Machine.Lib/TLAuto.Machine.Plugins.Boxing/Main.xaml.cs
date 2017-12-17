// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace TLAuto.Machine.Plugins.Boxing
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

        private void Second_Checked(object sender, RoutedEventArgs e)
        {
            OnBreakSecond();
        }

        private void Sencond_Unchecked(object sender, RoutedEventArgs e)
        {
            OnUnbreakSecond();
        }

        private void Third_Checked(object sender, RoutedEventArgs e)
        {
            OnBreakThird();
        }

        private void Third_Unchecked(object sender, RoutedEventArgs e)
        {
            OnUnbreakThird();
        }

        public event EventHandler BreakSecond;

        protected virtual void OnBreakSecond()
        {
            BreakSecond?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler UnbreakSecond;

        protected virtual void OnUnbreakSecond()
        {
            UnbreakSecond?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler BreakThird;

        protected virtual void OnBreakThird()
        {
            BreakThird?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler UnbreakThird;

        protected virtual void OnUnbreakThird()
        {
            UnbreakThird?.Invoke(this, EventArgs.Empty);
        }
    }
}