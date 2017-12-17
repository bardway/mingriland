// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.DMX.View.Views
{
    /// <summary>
    /// AddDMXDeviceView.xaml 的交互逻辑
    /// </summary>
    public partial class AddDMXDeviceView : Window
    {
        public AddDMXDeviceView()
        {
            InitializeComponent();
        }

        public string DMXDeviceName { private set; get; }

        public int ChannelBegin { private set; get; }

        public int ChannelEnd { private set; get; }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            var dmxDeviceName = TxtDMXDeviceName.Text.Trim();
            int channelBegin;
            int channelEnd;
            if (!dmxDeviceName.IsNullOrEmpty() && int.TryParse(TxtChannelBegin.Text, out channelBegin) && int.TryParse(TxtChannelEnd.Text, out channelEnd))
            {
                DMXDeviceName = dmxDeviceName;
                ChannelBegin = channelBegin;
                ChannelEnd = channelEnd;
                DialogResult = true;
                Close();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}