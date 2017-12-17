// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.NavFrame;
#endregion

namespace TLAuto.Device.ServerHost.Controls
{
    /// <summary>
    /// InputPLCSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class InputPLCSettingsPage : Page, INotifyNavStatusChanged
    {
        public InputPLCSettingsPage()
        {
            InitializeComponent();
            //CboDeviceTypes.ItemsSource = EnumExtensions.ToEnums<InputPLCDeviceType>();
            CboCaptureNumber.ItemsSource = GetCaptureNumber();
        }

        private IEnumerable<int> GetCaptureNumber()
        {
            var list = new List<int>();
            for (var i = 32; i > 0; i--)
            {
                list.Add(i);
            }
            return list;
        }

        private void CboDeviceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboCaptureNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtDeviceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void UpdateNavStatus()
        {
            int deviceNumber;
            var result1 = CboDeviceTypes.SelectedItem != null;
            var result2 = CboCaptureNumber.SelectedItem != null;
            var result3 = !TxtDeviceNumber.Text.IsNullOrEmpty() && int.TryParse(TxtDeviceNumber.Text, out deviceNumber);
            if (result1 && result2 && result3)
            {
                OnNavStatusChanged(new NavStatusChangedEventArgs(true));
            }
            else
            {
                OnNavStatusChanged(new NavStatusChangedEventArgs(false));
            }
            UpdateSettingsParam();
        }

        private void UpdateSettingsParam()
        {
            if (ParamObj != null)
            {
                var settingsParam = (InputPLCSettingsPageParam)ParamObj;
                //settingsParam.InputPLCDeviceType = (InputPLCDeviceType?)CboDeviceTypes.SelectedItem;
                settingsParam.DeviceCaptureNumber = (int?)CboCaptureNumber.SelectedItem ?? 0;
                int deviceNumber;
                int.TryParse(TxtDeviceNumber.Text, out deviceNumber);
                settingsParam.DeviceNumber = deviceNumber;
            }
        }

        #region INotifyNavStatusChanged
        public object ParamObj { set; get; }

        public event NavStatusChangedEventHandler NavStatusChanged;

        protected virtual void OnNavStatusChanged(NavStatusChangedEventArgs e)
        {
            NavStatusChanged?.Invoke(this, e);
        }
        #endregion
    }
}