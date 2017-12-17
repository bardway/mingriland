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
    /// EditBoardDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditBoardDialog : Window
    {
        public EditBoardDialog(string boardName, int deviceNumber, string serviceAddressMark, string signName, IList<ServiceAddressInfo> serviceInfos)
        {
            InitializeComponent();
            TxtBoardName.Text = boardName;
            TxtDeviceNumber.Text = deviceNumber.ToString();
            TxtSignName.Text = signName;
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

            Loaded += EditBoardDialog_Loaded;
        }

        public string BoardName => TxtBoardName.Text.ToTrim();

        public int DeviceNumber => TxtDeviceNumber.Text.ToInt32();

        public string SelectedServiceAddressMark => ((ServiceAddressInfo)CboServices.SelectedItem).Mark;

        public string SignName => TxtSignName.Text.Trim();

        private void EditBoardDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TxtBoardName.Focus();
            TxtBoardName.SelectAll();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!BoardName.IsNullOrEmpty())
            {
                if ((DeviceNumber >= 1) && (DeviceNumber <= 255))
                {
                    if (CboServices.SelectedIndex != -1)
                    {
                        if (!SignName.IsNullOrEmpty())
                        {
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("必须填写端口号。");
                        }
                    }
                    else
                    {
                        MessageBox.Show("必需选择通讯地址。");
                    }
                }
                else
                {
                    MessageBox.Show("设备号需满足1-255范围。");
                }
            }
            else
            {
                MessageBox.Show("参数填写错误。");
            }
        }
    }
}