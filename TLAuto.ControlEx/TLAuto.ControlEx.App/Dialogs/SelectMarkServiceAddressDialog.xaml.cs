// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// SelectMarkServiceAddressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectMarkServiceAddressDialog : Window
    {
        public SelectMarkServiceAddressDialog(IEnumerable<string> marks)
        {
            InitializeComponent();
            CboMarks.ItemsSource = marks;
            CboMarks.SelectedIndex = 0;
        }

        public string SelectedMark
        {
            get
            {
                if (CboMarks.SelectedItem == null)
                {
                    return string.Empty;
                }
                return (string)CboMarks.SelectedItem;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedMark.IsNullOrEmpty())
            {
                DialogResult = true;
            }
            Close();
        }
    }
}