// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Input;
#endregion

namespace TLAuto.BaseEx.Mvvm
{
    public class RelayKeyBinding : KeyBinding
    {
        public static readonly DependencyProperty CommandBindingProperty =
            DependencyProperty.Register("CommandBinding", typeof(ICommand), typeof(RelayKeyBinding), new FrameworkPropertyMetadata(OnCommandBindingChanged));

        public ICommand CommandBinding { get => (ICommand)GetValue(CommandBindingProperty); set => SetValue(CommandBindingProperty, value); }

        private static void OnCommandBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBinding = (RelayKeyBinding)d;
            keyBinding.Command = (ICommand)e.NewValue;
        }
    }
}