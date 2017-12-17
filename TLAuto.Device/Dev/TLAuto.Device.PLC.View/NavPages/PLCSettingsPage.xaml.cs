// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.NavFrame;
#endregion

namespace TLAuto.Device.PLC.View.NavPages
{
    /// <summary>
    /// PLCSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class PLCSettingsPage : Page, INotifyNavStatusChanged
    {
        private bool _isLoaded;

        public PLCSettingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                return;
            }
            _isLoaded = true;
            var settingsParam = (PLCSettingsInfo)ParamObj;
            if (settingsParam.IsEdit)
            {
                TxtPLCHeaderName.Text = settingsParam.Current.PLCHeaderName;
                TxtDeviceNumber.Text = settingsParam.Current.DeviceNumber.ToString();
                TxtDigitalSwitchNumber.Text = settingsParam.Current.DigitalSwitchNumber.ToString();
                TxtRelayNumber.Text = settingsParam.Current.RelayNumber.ToString();
                CboTypes.ItemsSource = new List<PLCDeviceType> {settingsParam.Current.PLCDeviceType};
                CboTypes.SelectedIndex = 0;
                CboTypes.IsEnabled = false;
            }
            else
            {
                CboTypes.ItemsSource = EnumExtensions.ToEnums<PLCDeviceType>();
            }
            UpdateNavStatus();
        }

        private void CboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboControlRelayResultStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtPLCHeaderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtDeviceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtDigitalSwitchNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtRelayNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void UpdateNavStatus()
        {
            if (!IsLoaded)
            {
                return;
            }
            int deviceNumber;
            int dswitchNumber;
            int relayNumber;
            var result1 = CboTypes.SelectedItem != null;
            var result2 = !TxtPLCHeaderName.Text.IsNullOrEmpty();
            var result3 = int.TryParse(TxtDeviceNumber.Text, out deviceNumber);
            var result4 = int.TryParse(TxtDigitalSwitchNumber.Text, out dswitchNumber);
            var result5 = int.TryParse(TxtRelayNumber.Text, out relayNumber);
            var result6 = CboControlRelayResultStatus.SelectedValue != null;
            OnNavStatusChanged(result1 && result2 && result3 && result4 && result5 && result6 ? new NavStatusChangedEventArgs(true) : new NavStatusChangedEventArgs(false));
            UpdateSettingsParam();
        }

        private void UpdateSettingsParam()
        {
            if (ParamObj != null)
            {
                var settingsParam = (PLCSettingsInfo)ParamObj;
                settingsParam.Current.PLCDeviceType = (PLCDeviceType)(CboTypes.SelectedItem?.ToInt32() ?? 0);
                settingsParam.Current.PLCHeaderName = TxtPLCHeaderName.Text.Trim();
                if (!TxtDeviceNumber.Text.IsNullOrEmpty())
                {
                    settingsParam.Current.DeviceNumber = TxtDeviceNumber.Text.ToInt32();
                }
                if (!TxtDigitalSwitchNumber.Text.IsNullOrEmpty())
                {
                    settingsParam.Current.DigitalSwitchNumber = TxtDigitalSwitchNumber.Text.ToInt32();
                }
                if (!TxtRelayNumber.Text.IsNullOrEmpty())
                {
                    settingsParam.Current.RelayNumber = TxtRelayNumber.Text.ToInt32();
                }
                settingsParam.Current.HasControlRelayResultStatus = CboControlRelayResultStatus.SelectedValue?.ToBoolean() ?? false;
            }
        }

        #region INavContent
        public object ParamObj { set; get; }

        public event NavStatusChangedEventHandler NavStatusChanged;

        protected virtual void OnNavStatusChanged(NavStatusChangedEventArgs e)
        {
            NavStatusChanged?.Invoke(this, e);
        }
        #endregion
    }
}