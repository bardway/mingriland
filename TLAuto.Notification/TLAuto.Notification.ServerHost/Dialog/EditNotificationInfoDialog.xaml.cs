// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Linq;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.Notification.Contracts;
#endregion

namespace TLAuto.Notification.ServerHost.Dialog
{
    /// <summary>
    /// EditNotificationInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditNotificationInfoDialog : Window
    {
        public EditNotificationInfoDialog()
        {
            InitializeComponent();
            var names = Enum.GetNames(typeof(AppStatusType));
            var list = names.Select(name => (AppStatusType)Enum.Parse(typeof(AppStatusType), name)).ToList();
            CboStatus.ItemsSource = list;
            CboStatus.SelectedIndex = 0;
        }

        public string AppKey => TxtAppKey.Text.Trim();

        public AppStatusType SelectedAppStatusType => (AppStatusType)CboStatus.SelectedItem;

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