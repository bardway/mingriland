// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
#endregion

namespace TLAuto.BaseEx.AttachedPropertys
{
    public class AttachedWindowStyle
    {
        public static readonly DependencyProperty HasCenterScreenProperty =
            DependencyProperty.RegisterAttached("HasCenterScreen",
                                                typeof(bool),
                                                typeof(AttachedWindowStyle),
                                                new FrameworkPropertyMetadata(false,
                                                                              (sender, e) =>
                                                                              {
                                                                                  var window = sender as Window;
                                                                                  if (window == null)
                                                                                  {
                                                                                      return;
                                                                                  }
                                                                                  if ((bool)e.NewValue)
                                                                                  {
                                                                                      window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                                                                  }
                                                                              }));

        public static bool GetHasCenterScreen(UIElement element)
        {
            return (bool)element.GetValue(HasCenterScreenProperty);
        }

        public static void SetHasCenterScreen(UIElement element, bool hasDialog)
        {
            element.SetValue(HasCenterScreenProperty, hasDialog);
        }
    }
}