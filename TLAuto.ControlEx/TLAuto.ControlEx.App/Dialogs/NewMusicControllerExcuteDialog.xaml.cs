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
    /// NewMusicControllerExcuteDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewMusicControllerExcuteDialog : Window
    {
        public NewMusicControllerExcuteDialog()
        {
            InitializeComponent();
            var names = Enum.GetNames(typeof(MusicControlType));
            foreach (var name in names)
            {
                CboExcutes.Items.Add((MusicControlType)Enum.Parse(typeof(MusicControlType), name));
            }
            CboExcutes.SelectedIndex = 0;
        }

        public MusicControlType SelectedMusicExcuteType => (MusicControlType)CboExcutes.SelectedItem;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CboExcutes.SelectedIndex != -1)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}