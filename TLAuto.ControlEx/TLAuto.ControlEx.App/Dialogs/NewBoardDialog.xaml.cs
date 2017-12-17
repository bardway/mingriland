// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// NewBoardDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewBoardDialog : Window
    {
        public NewBoardDialog(BoardType boardType, string serviceAddressMark, IList<ServiceAddressInfo> serviceInfos)
        {
            InitializeComponent();
            switch (boardType)
            {
                case BoardType.InputA:
                    TblItemCountTitle.Text = "工业版Switch数量";
                    break;
                case BoardType.OutputA:
                    TblItemCountTitle.Text = "工业版Relay数量";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("boardType", boardType, null);
            }
            for (var i = 0; i < 32; i++)
            {
                CboItemCount.Items.Add(i + 1);
            }
            CboItemCount.SelectedIndex = 31;

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

            Loaded += NewBoardDialog_Loaded;
        }

        public int ItemCount => (int)CboItemCount.SelectedItem;

        public string BoardName => TxtBoardName.Text.ToTrim();

        public int DeviceNumber => TxtDeviceNumber.Text.ToInt32();

        public string SelectedServiceAddressMark => ((ServiceAddressInfo)CboServices.SelectedItem).Mark;

        public string SignName => TxtSignName.Text.Trim();

        private void NewBoardDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TxtBoardName.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if ((CboItemCount.SelectedIndex != -1) && !BoardName.IsNullOrEmpty())
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