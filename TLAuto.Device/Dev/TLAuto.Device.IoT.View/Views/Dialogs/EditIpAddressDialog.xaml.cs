// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.Device.IoT.View.Models.Enums;
#endregion

namespace TLAuto.Device.IoT.View.Views.Dialogs
{
    /// <summary>
    /// EditIpAddressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditIpAddressDialog : Window
    {
        public EditIpAddressDialog(string ip, int port, string signName, int deviceNumber, string deviceHeader, IoTSocketType iotSocketType)
        {
            InitializeComponent();
            Ip = ip;
            Port = port;
            SignName = signName;
            DeviceNumber = deviceNumber;
            DeviceHeader = deviceHeader;
            IoTScoketType = iotSocketType;
        }

        public string Ip { set => TxtIp.Text = value; get => TxtIp.Text.Trim(); }

        public int Port { set => TxtPort.Text = value.ToString(); get => TxtPort.Text.Trim().ToInt32(); }

        public string SignName { set => TxtSignName.Text = value; get => TxtSignName.Text.Trim(); }

        public int DeviceNumber { set => TxtDeviceNumber.Text = value.ToString(); get => TxtDeviceNumber.Text.Trim().ToInt32(); }

        public string DeviceHeader { set => TxtDeviceHeader.Text = value; get => TxtDeviceHeader.Text.Trim(); }

        public IoTSocketType IoTScoketType { set => CboTypes.SelectedValue = value; get => (IoTSocketType)Enum.Parse(typeof(IoTSocketType), CboTypes.SelectedValue.ToString()); }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Ip.IsNullOrEmpty() && (Port != 0) && (DeviceNumber >= 0) && !SignName.IsNullOrEmpty() && !DeviceHeader.IsNullOrEmpty())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}