// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditMarkServiceAddressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditMarkServiceAddressDialog : Window
    {
        public EditMarkServiceAddressDialog(string mark, string serviceAddressMark, IList<ServiceAddressInfo> serviceInfos)
        {
            InitializeComponent();
            TxtMark.Text = mark;
            CboServices.ItemsSource = serviceInfos;
            var serviceInfo = serviceInfos.FirstOrDefault(s => s.Mark == serviceAddressMark);
            if (serviceInfo != null)
            {
                CboServices.SelectedItem = serviceInfo;
            }
            else
            {
                CboServices.SelectedIndex = 0;
            }
        }

        public string Mark => TxtMark.Text.Trim();

        public string SelectedServiceAddressMark => ((ServiceAddressInfo)CboServices.SelectedItem).Mark;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Mark.IsNullOrEmpty() && (CboServices.SelectedIndex != -1))
            {
                DialogResult = true;
                Close();
            }
        }
    }
}