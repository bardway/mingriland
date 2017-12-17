// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditServiceAddressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditServiceAddressDialog : Window
    {
        private readonly ServiceAddressInfo _serviceInfo;

        public EditServiceAddressDialog(ServiceAddressInfo serviceInfo)
        {
            InitializeComponent();
            _serviceInfo = serviceInfo;
            TxtMark.Text = serviceInfo.Mark;
            TxtServiceAddress.Text = serviceInfo.ServiceAddress;
        }

        private string Mark => TxtMark.Text.Trim();

        private string ServiceAddress => TxtServiceAddress.Text.Trim();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Mark.IsNullOrEmpty() && !ServiceAddress.IsNullOrEmpty())
            {
                _serviceInfo.Mark = Mark;
                _serviceInfo.ServiceAddress = ServiceAddress;
                DialogResult = true;
                Close();
            }
        }
    }
}