// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditMusicVolumeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditMusicVolumeDialog : Window
    {
        public EditMusicVolumeDialog()
        {
            InitializeComponent();
            var names = Enum.GetNames(typeof(MusicControlType));
            foreach (var name in names)
            {
                var enumType = (MusicControlType)Enum.Parse(typeof(MusicControlType), name);
                if ((enumType != MusicControlType.AdjustVolume) && (enumType != MusicControlType.Pause))
                {
                    CboMusicExcutes.Items.Add(enumType);
                }
            }
            CboMusicExcutes.SelectedIndex = 0;
        }

        public double Volume => SliderControl.Value;

        public MusicControlType SelectedMusicControlType => (MusicControlType)CboMusicExcutes.SelectedItem;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}