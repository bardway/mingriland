// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Music.ServerHost.Dialog
{
    /// <summary>
    /// EditMusicMarkDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditMusicMarkDialog : Window
    {
        public EditMusicMarkDialog(IEnumerable<string> marks)
        {
            InitializeComponent();
            CboMarks.ItemsSource = marks;
            CboMarks.SelectedIndex = 0;
        }

        public string Mark => CboMarks.SelectedItem == null ? string.Empty : CboMarks.SelectedItem.ToString();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Mark.IsNullOrEmpty())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}