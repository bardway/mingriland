// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.ServerHost.Views
{
    /// <summary>
    /// EditServiceInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class EditServiceInfoView : Window
    {
        public EditServiceInfoView(string serviceAddress, bool isAutoStart)
        {
            InitializeComponent();
            TxtServiceAddress.Text = serviceAddress;
            CboControl.SelectedIndex = isAutoStart.ToInt32();
        }

        public string ServiceAddress => TxtServiceAddress.Text.Trim();

        public bool IsAutoStart => CboControl.SelectedValue.ToBoolean();

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (!TxtServiceAddress.Text.IsNullOrEmpty() && (CboControl.SelectedValue != null))
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("信息有错误，请更正。");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}