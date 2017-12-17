// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Controls;

using TLAuto.ControlEx.App.Controls.Models.Events;
#endregion

namespace TLAuto.ControlEx.App.Controls
{
    /// <summary>
    /// EditTextControlEx.xaml 的交互逻辑
    /// </summary>
    public partial class EditTextControlEx : UserControl
    {
        public EditTextControlEx()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            BtnEdit.Visibility = Visibility.Collapsed;
            BtnUpdate.Visibility = Visibility.Visible;
            EditControl.IsEditText = true;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            EditControl.RenameText();
        }

        private void EditControl_EditTextChanged(object sender, EditTextChangedRoutedEventArgs e)
        {
            Text = e.EditText;
            BtnEdit.Visibility = Visibility.Visible;
            BtnUpdate.Visibility = Visibility.Collapsed;
        }

        #region DependencyProperties
        #region Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditTextControlEx), new PropertyMetadata(string.Empty, OnTextChanged));

        private void OnTextChanged(string newText)
        {
            if (EditControl.Text != newText)
            {
                EditControl.Text = newText;
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditTextControlEx)d).OnTextChanged((string)e.NewValue);
        }

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        #endregion
        #endregion
    }
}