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

namespace TLAuto.Device.Projector.View.NavPages
{
    /// <summary>
    /// ProjectorsSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectorSettingsPage : Page, INotifyNavStatusChanged
    {
        private bool _isLoaded;

        public ProjectorSettingsPage()
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
            var settingsParam = (ProjectorSettingsInfo)ParamObj;
            if (settingsParam.IsEdit)
            {
                TxtProjectorHeaderName.Text = settingsParam.Current.ProjectorHeaderName;
                TxtDeviceNumber.Text = settingsParam.Current.DeviceNumber.ToString();
                CboTypes.ItemsSource = new List<ProjectorDeviceType> {settingsParam.Current.ProjectorDeviceType};
                CboTypes.SelectedIndex = 0;
                CboTypes.IsEnabled = false;
            }
            else
            {
                CboTypes.ItemsSource = EnumExtensions.ToEnums<ProjectorDeviceType>();
            }
            UpdateNavStatus();
        }

        private void CboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtProjectorHeaderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void TxtDeviceNumber_TextChanged(object sender, TextChangedEventArgs e)
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
            var result1 = CboTypes.SelectedItem != null;
            var result2 = !TxtProjectorHeaderName.Text.IsNullOrEmpty();
            var result3 = int.TryParse(TxtDeviceNumber.Text, out deviceNumber);
            OnNavStatusChanged(result1 && result2 && result3 ? new NavStatusChangedEventArgs(true) : new NavStatusChangedEventArgs(false));
            UpdateSettingsParam();
        }

        private void UpdateSettingsParam()
        {
            if (ParamObj != null)
            {
                var settingsParam = (ProjectorSettingsInfo)ParamObj;
                settingsParam.Current.ProjectorDeviceType = (ProjectorDeviceType)(CboTypes.SelectedItem?.ToInt32() ?? 0);
                settingsParam.Current.ProjectorHeaderName = TxtProjectorHeaderName.Text.Trim();
                if (!TxtDeviceNumber.Text.IsNullOrEmpty())
                {
                    settingsParam.Current.DeviceNumber = TxtDeviceNumber.Text.ToInt32();
                }
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