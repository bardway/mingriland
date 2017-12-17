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
    /// EditAppNameDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditAppNameDialog : Window
    {
        public EditAppNameDialog()
        {
            InitializeComponent();
        }

        public string ProcessName => TxtProcessName.Text.Trim();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ProcessName.IsNullOrEmpty())
            {
                DialogResult = true;
            }
            Close();
        }
    }
}