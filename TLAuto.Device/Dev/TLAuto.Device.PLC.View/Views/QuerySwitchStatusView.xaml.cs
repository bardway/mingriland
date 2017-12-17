// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.PLC.View.Views
{
    /// <summary>
    /// QuerySwitchStatusView.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySwitchStatusView : Window
    {
        public QuerySwitchStatusView()
        {
            InitializeComponent();
        }

        public int[] SwitchNums { private set; get; }

        public bool IsAutoUpload { private set; get; }

        private void CboQueryTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsAutoUpload = CboQueryTypes.SelectedValue.ToBoolean();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var switchNums = TxtSwitchNumber.Text.Split('|');
                var switchList = switchNums.Select(switchNum => switchNum.ToInt32()).ToArray();
                SwitchNums = switchList;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}