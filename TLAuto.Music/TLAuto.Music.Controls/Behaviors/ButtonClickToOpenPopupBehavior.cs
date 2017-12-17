// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
#endregion

namespace TLAuto.Music.Controls.Behaviors
{
    public class ButtonClickToOpenPopupBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty IsOpenPopupProperty =
            DependencyProperty.Register("IsOpenPopup", typeof(bool), typeof(ButtonClickToOpenPopupBehavior));

        public bool IsOpenPopup { get => (bool)GetValue(IsOpenPopupProperty); set => SetValue(IsOpenPopupProperty, value); }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            IsOpenPopup = !IsOpenPopup;
        }
    }
}