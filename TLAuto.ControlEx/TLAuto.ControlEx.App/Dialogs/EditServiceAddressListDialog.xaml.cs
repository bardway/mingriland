// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditServiceAddressListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditServiceAddressListDialog : Window
    {
        private readonly ObservableCollection<ServiceAddressInfo> _editServiceAddressInfos;

        public EditServiceAddressListDialog(ObservableCollection<ServiceAddressInfo> editServiceAddressInfos)
        {
            InitializeComponent();
            _editServiceAddressInfos = editServiceAddressInfos;
            LstServiceAdreess.ItemsSource = _editServiceAddressInfos;
        }

        private string Mark => TxtMark.Text.Trim();

        private string ServiceAddress => TxtServiceAddress.Text.Trim();

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!Mark.IsNullOrEmpty() && !ServiceAddress.IsNullOrEmpty())
            {
                var serviceAddressInfo = new ServiceAddressInfo
                                         {
                                             Mark = Mark,
                                             ServiceAddress = ServiceAddress
                                         };
                _editServiceAddressInfos.Add(serviceAddressInfo);
                TxtMark.Text = string.Empty;
                TxtServiceAddress.Text = string.Empty;
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LstServiceAdreess.SelectedItem != null)
            {
                _editServiceAddressInfos.Remove((ServiceAddressInfo)LstServiceAdreess.SelectedItem);
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LstServiceAdreess.SelectedItem != null)
            {
                var serviceInfo = (ServiceAddressInfo)LstServiceAdreess.SelectedItem;
                var ead = new EditServiceAddressDialog(serviceInfo);
                ead.ShowDialog();
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}