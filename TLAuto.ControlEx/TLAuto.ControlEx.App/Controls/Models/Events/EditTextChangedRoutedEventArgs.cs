// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
#endregion

namespace TLAuto.ControlEx.App.Controls.Models.Events
{
    public class EditTextChangedRoutedEventArgs : RoutedEventArgs
    {
        public EditTextChangedRoutedEventArgs(RoutedEvent routedEvent, object source, string editText, string oldText)
            : base(routedEvent, source)
        {
            EditText = editText;
            OldText = oldText;
        }

        public string EditText { get; }

        public string OldText { get; }
    }
}