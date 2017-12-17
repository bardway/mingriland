// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Forms;

using TLAuto.Base.Extensions;

using MessageBox = System.Windows.MessageBox;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// NewPorjectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        public NewProjectDialog()
        {
            InitializeComponent();
            ContentRendered += NewProjectDialog_ContentRendered;
        }

        public string FileName => TxtFileName.Text.Trim();

        public string Location => TxtLocation.Text;

        private void NewProjectDialog_ContentRendered(object sender, EventArgs e)
        {
            TxtFileName.Focus();
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!fbd.SelectedPath.IsNullOrEmpty())
                {
                    TxtLocation.Text = fbd.SelectedPath;
                }
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (!FileName.IsNullOrEmpty() && !Location.IsNullOrEmpty())
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("参数格式不正确。");
            }
        }
    }
}