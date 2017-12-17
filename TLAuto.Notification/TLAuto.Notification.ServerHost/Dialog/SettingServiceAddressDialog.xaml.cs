// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Notification.ServerHost.Dialog
{
    /// <summary>
    /// SettingServiceAddressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SettingServiceAddressDialog : Window
    {
        public SettingServiceAddressDialog(string serviceAddress)
        {
            InitializeComponent();
            if (!serviceAddress.IsNullOrEmpty())
            {
                TxtServiceAddress.Text = serviceAddress;
            }
        }

        public string ServiceAddress => TxtServiceAddress.Text.Trim();

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (TxtServiceAddress.Focus())
            {
                TxtServiceAddress.SelectAll();
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ServiceAddress.IsNullOrEmpty())
            {
                DialogResult = true;
            }
            Close();
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(ServiceAddress);
        }
    }
}