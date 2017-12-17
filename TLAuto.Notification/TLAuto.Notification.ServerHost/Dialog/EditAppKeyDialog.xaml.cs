// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Notification.ServerHost.Dialog
{
    /// <summary>
    /// EditAppKeyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditAppKeyDialog : Window
    {
        public EditAppKeyDialog()
        {
            InitializeComponent();
        }

        public string AppKey => TxtAppKey.Text.Trim();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppKey.IsNullOrEmpty())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}