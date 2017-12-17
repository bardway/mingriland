// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Controls.Models.Events;
#endregion

namespace TLAuto.ControlEx.App.Controls
{
    /// <summary>
    /// EditTextControl.xaml 的交互逻辑
    /// </summary>
    public partial class EditTextControl : UserControl
    {
        #region Ctors
        public EditTextControl()
        {
            InitializeComponent();
        }
        #endregion

        public void RenameText()
        {
            if (!IsEditText)
            {
                return;
            }
            IsEditText = false;
            var changeText = TxtEditHeader.Text.Trim();
            if (changeText.IsNullOrEmpty())
            {
                MessageBox.Show("必须输入名称。");
                IsEditText = true;
            }
            else
            {
                var oldText = Text;
                Text = changeText;
                RaiseEditTextChangedEvent(Text, oldText);
            }
        }

        private void EditHeaderTextSelect()
        {
            if (IsSelectAllWithoutExtension)
            {
                if (Path.HasExtension(Text))
                {
                    TxtEditHeader.Select(0, Path.GetFileNameWithoutExtension(Text).Length);
                }
                else
                {
                    TxtEditHeader.SelectAll();
                }
            }
            else
            {
                TxtEditHeader.SelectAll();
            }
        }

        private void TxtEditHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RenameText();
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    IsEditText = false;
                    RaiseEditTextChangedEvent(Text, Text);
                }
            }
        }

        private void TxtEditHeader_LostFocus(object sender, RoutedEventArgs e)
        {
            RenameText();
        }

        private void TxtEditHeader_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            RenameText();
        }

        private void TxtEditHeader_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsEditText)
            {
                if (TxtEditHeader.Focus())
                {
                    EditHeaderTextSelect();
                }
            }
        }

        #region DependencyProperties
        #region IsEditText
        public static readonly DependencyProperty IsEditTextProperty =
            DependencyProperty.Register("IsEditText", typeof(bool), typeof(EditTextControl), new PropertyMetadata(false, OnIsEditTextChanged));

        private void OnIsEditTextChanged(bool isEditText)
        {
            if (isEditText)
            {
                TblHeader.Visibility = Visibility.Collapsed;
                TxtEditHeader.Visibility = Visibility.Visible;
                if (TxtEditHeader.Focus())
                {
                    EditHeaderTextSelect();
                }
            }
            else
            {
                TblHeader.Visibility = Visibility.Visible;
                TxtEditHeader.Visibility = Visibility.Collapsed;
            }
        }

        private static void OnIsEditTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditTextControl)d).OnIsEditTextChanged((bool)e.NewValue);
        }

        public bool IsEditText { get => (bool)GetValue(IsEditTextProperty); set => SetValue(IsEditTextProperty, value); }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditTextControl), new PropertyMetadata(string.Empty, OnTextChanged));

        private void OnTextChanged(string newText)
        {
            TblHeader.Text = newText;
            TxtEditHeader.Text = newText;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditTextControl)d).OnTextChanged((string)e.NewValue);
        }

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        #endregion

        #region IsSelectAllWithoutExtension
        public static readonly DependencyProperty IsSelectAllWithoutExtensionProperty =
            DependencyProperty.Register("IsSelectAllWithoutExtension", typeof(bool), typeof(EditTextControl), new PropertyMetadata(false));

        public bool IsSelectAllWithoutExtension { get => (bool)GetValue(IsSelectAllWithoutExtensionProperty); set => SetValue(IsSelectAllWithoutExtensionProperty, value); }
        #endregion
        #endregion

        #region Routed Events
        #region EditTextChanged
        public static readonly RoutedEvent EditTextChangedEvent =
            EventManager.RegisterRoutedEvent("EditTextChanged", RoutingStrategy.Bubble, typeof(EventHandler<EditTextChangedRoutedEventArgs>), typeof(EditTextControl));

        public event EventHandler<EditTextChangedRoutedEventArgs> EditTextChanged { add => AddHandler(EditTextChangedEvent, value); remove => RemoveHandler(EditTextChangedEvent, value); }

        private void RaiseEditTextChangedEvent(string changeText, string oldText)
        {
            var args = new EditTextChangedRoutedEventArgs(EditTextChangedEvent, this, changeText, oldText);
            RaiseEvent(args);
        }
        #endregion
        #endregion
    }
}