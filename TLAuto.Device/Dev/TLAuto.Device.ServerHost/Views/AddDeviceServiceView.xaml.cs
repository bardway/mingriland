// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.ServerHost.Views
{
    /// <summary>
    /// AddDeviceServiceDialogView.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeviceServiceDialogView
    {
        public AddDeviceServiceDialogView()
        {
            InitializeComponent();
            var deviceServices = new ObservableCollection<Tuple<string, string>>();
            CboControl.ItemsSource = deviceServices;
            var services = TLDeviceExtensionsService.Instance.DeviceServices.Where(s => !s.DeviceSettings.Exists).Select(s => new Tuple<string, string>(s.Description, s.ServiceKey)).ToList();
            foreach (var service in services)
            {
                deviceServices.Add(service);
            }
        }

        public string ServiceKey => CboControl.SelectedItem != null ? ((Tuple<string, string>)CboControl.SelectedItem).Item2 : string.Empty;

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (CboControl.SelectedItem != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("设备通信类型不能为空。");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}