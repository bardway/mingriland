// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Music.ServerHost.Dialog
{
    /// <summary>
    /// EditMusicVolumeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditMusicVolumeDialog : Window
    {
        private readonly bool _isAdjustVolumeAll;

        public EditMusicVolumeDialog(bool isAdjustVolumeAll, IEnumerable<string> marks)
        {
            InitializeComponent();
            CboMarks.ItemsSource = marks;
            CboMarks.SelectedIndex = 0;
            _isAdjustVolumeAll = isAdjustVolumeAll;
            if (_isAdjustVolumeAll)
            {
                TblMarkTitle.Visibility = Visibility.Collapsed;
                CboMarks.Visibility = Visibility.Collapsed;
            }
        }

        public double Volume => SliderVolume.Value;

        public string Mark => CboMarks.SelectedItem == null ? string.Empty : CboMarks.SelectedItem.ToString();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdjustVolumeAll)
            {
                if (Mark.IsNullOrEmpty())
                {
                    return;
                }
            }
            DialogResult = true;
            Close();
        }
    }
}