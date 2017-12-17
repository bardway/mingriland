// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
#endregion

namespace TLAuto.Device.Controls.NavFrame
{
    /// <summary>
    /// NavFrameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NavFrameWindow : Window
    {
        #region Ctors
        public NavFrameWindow(IEnumerable<NavFrameInfo> navFrames, string navTitle)
        {
            InitializeComponent();
            Title = navTitle;
            _navFrameInfos = (navFrames as NavFrameInfo[] ?? navFrames.ToArray()).ToList();
            NavItemsControl.ItemsSource = _navFrameInfos;
            foreach (var navFrameInfo in _navFrameInfos)
            {
                navFrameInfo.NavCompleted += NavFrameInfo_NavCompleted;
                navFrameInfo.NavFailed += NavFrameInfo_NavFailed;
            }
            if (_navFrameInfos.Count > 0)
            {
                _navFrameInfos[0].Show(DeviceFrame);
                if (_navFrameInfos.Count == 1)
                {
                    NavBackButton.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        private void DeviceFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var navFrameInfo = _navFrameInfos.FirstOrDefault(s => s.IsShow);
            if (navFrameInfo != null)
            {
                _selectedNavFrameInfo = navFrameInfo;
                UpdateForwardStatus(_selectedNavFrameInfo);
            }
        }

        private void NavFrameInfo_NavFailed(object sender, EventArgs e)
        {
            var navFrameInfo = (NavFrameInfo)sender;
            var index = _navFrameInfos.IndexOf(navFrameInfo);
            if (index != -1)
            {
                NavForwardButton.IsEnabled = false;
            }
        }

        private void NavFrameInfo_NavCompleted(object sender, EventArgs e)
        {
            var navFrameInfo = (NavFrameInfo)sender;
            UpdateForwardStatus(navFrameInfo);
        }

        private bool IsEndNavFrameInfo(NavFrameInfo navFrameInfo)
        {
            var index = _navFrameInfos.IndexOf(navFrameInfo);
            return index == _navFrameInfos.Count - 1;
        }

        private void UpdateForwardStatus(NavFrameInfo navFrameInfo)
        {
            NavForwardButton.IsEnabled = navFrameInfo.HasCompletedSettings;
            var isEndNavStatus = IsEndNavFrameInfo(navFrameInfo);
            NavForwardButton.Content = isEndNavStatus ? "确定" : "下一步";
        }

        private void NavBack_Click(object sender, RoutedEventArgs e)
        {
            DeviceFrame.GoBack();
        }

        private void NavForward_Click(object sender, RoutedEventArgs e)
        {
            var isEndNavStatus = IsEndNavFrameInfo(_selectedNavFrameInfo);
            if (!isEndNavStatus)
            {
                var index = _navFrameInfos.IndexOf(_selectedNavFrameInfo);
                if (DeviceFrame.CanGoForward)
                {
                    DeviceFrame.GoForward();
                }
                else
                {
                    _navFrameInfos[index + 1].Show(DeviceFrame);
                }
            }
            else
            {
                DialogResult = true;
                Close();
            }
        }

        #region Fields
        private readonly List<NavFrameInfo> _navFrameInfos;
        private NavFrameInfo _selectedNavFrameInfo;
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty NavTitleContentBackgroundProperty =
            DependencyProperty.Register("NavTitleContentBackground", typeof(Brush), typeof(NavFrameWindow), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Brush NavTitleContentBackground { get => (Brush)GetValue(NavTitleContentBackgroundProperty); set => SetValue(NavTitleContentBackgroundProperty, value); }

        public static readonly DependencyProperty NavFrameContentBackgroundProperty =
            DependencyProperty.Register("NavFrameContentBackground", typeof(Brush), typeof(NavFrameWindow), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Brush NavFrameContentBackground { get => (Brush)GetValue(NavFrameContentBackgroundProperty); set => SetValue(NavFrameContentBackgroundProperty, value); }
        #endregion
    }
}