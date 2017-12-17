// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings
{
    /// <summary>
    /// SerialPortSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortSettingsPage : Page, INotifyNavStatusChanged
    {
        private bool _isLoaded;

        public SerialPortSettingsPage()
        {
            InitializeComponent();
            TxtPortSignName.Text = Guid.NewGuid().ToString().Replace("-", "");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                return;
            }
            _isLoaded = true;
            var settingsParam = (SerialPortSettingsInfo)ParamObj;
            var usedPortNames = settingsParam.UsedSerialPortInfos.Select(s => s.Item1.PortName).ToList();
            var notUsedPortNames = SerialPortSettingsHelper.GetSerialPortNames(usedPortNames);
            var portNames = notUsedPortNames.Select(portName => new Tuple<string, bool>(portName, false)).ToList();
            portNames.AddRange(settingsParam.UsedSerialPortInfos.Select(usedInfo => new Tuple<string, bool>(usedInfo.Item1.PortName, usedInfo.Item2)));
            portNames.Sort(new Reverser<Tuple<string, bool>>(typeof(Tuple<string, bool>), "Item1", ReverserInfo.Direction.ASC));
            CboPortNames.ItemsSource = portNames;
            if (settingsParam.IsEdit)
            {
                var serialPortInfo = portNames.Find(s => s.Item2);
                var selectedIndex = portNames.IndexOf(serialPortInfo);
                CboPortNames.SelectedIndex = selectedIndex;
            }
            UpdateNavStatus();
        }

        private void TxtPortSignName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboPortNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var settingsParam = (SerialPortSettingsInfo)ParamObj;
                var portInfo = (Tuple<string, bool>)e.AddedItems[0];
                if (portInfo.Item2)
                {
                    var usedPortInfo = settingsParam.UsedSerialPortInfos.FirstOrDefault(s => s.Item1.PortName == portInfo.Item1);
                    if (usedPortInfo != null)
                    {
                        TxtPortSignName.Text = usedPortInfo.Item1.PortSignName;
                        CboBaudRates.SelectedItem = usedPortInfo.Item1.BaudRates;
                        CboDataBits.SelectedItem = usedPortInfo.Item1.DataBits;
                        CboParitys.SelectedItem = usedPortInfo.Item1.Parity;
                        CboStopBits.SelectedItem = usedPortInfo.Item1.StopBits;
                    }
                }

                TxtPortSignName.IsEnabled = !portInfo.Item2 && !portInfo.Item2;
                CboBaudRates.IsEnabled = !portInfo.Item2 && !portInfo.Item2;
                CboDataBits.IsEnabled = !portInfo.Item2 && !portInfo.Item2;
                CboStopBits.IsEnabled = !portInfo.Item2 && !portInfo.Item2;
                CboParitys.IsEnabled = !portInfo.Item2 && !portInfo.Item2;
            }
            UpdateNavStatus();
        }

        private void CboBaudRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboDataBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboParitys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void CboStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavStatus();
        }

        private void UpdateNavStatus()
        {
            if (!IsLoaded)
            {
                return;
            }
            var result1 = !TxtPortSignName.Text.IsNullOrEmpty();
            var result2 = (CboPortNames.SelectedItem != null) && !((Tuple<string, bool>)CboPortNames.SelectedItem).Item2;
            var result3 = CboBaudRates.SelectedItem != null;
            var result4 = CboDataBits.SelectedItem != null;
            var result5 = CboParitys.SelectedItem != null;
            var result6 = CboStopBits.SelectedItem != null;
            if (result1 && result2 && result3 && result4 && result5 && result6)
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
                var settingsParam = (SerialPortSettingsInfo)ParamObj;
                settingsParam.Current.Item1.PortSignName = TxtPortSignName.Text.Trim();
                settingsParam.Current.Item1.PortName = CboPortNames.SelectedItem == null ? string.Empty : ((Tuple<string, bool>)CboPortNames.SelectedItem).Item1;
                settingsParam.Current.Item1.BaudRates = CboBaudRates.SelectedItem?.ToInt32() ?? 0;
                settingsParam.Current.Item1.DataBits = CboDataBits.SelectedItem?.ToInt32() ?? 0;
                settingsParam.Current.Item1.Parity = (Parity)(CboParitys.SelectedItem?.ToInt32() ?? 0);
                settingsParam.Current.Item1.StopBits = (StopBits)(CboStopBits.SelectedItem?.ToInt32() ?? 0);
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