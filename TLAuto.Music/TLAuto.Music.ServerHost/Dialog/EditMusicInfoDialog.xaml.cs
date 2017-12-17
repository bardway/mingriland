// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using Microsoft.Win32;

using TLAuto.Base.Extensions;
using TLAuto.Music.ServerHost.Dialog.Models;
#endregion

namespace TLAuto.Music.ServerHost.Dialog
{
    /// <summary>
    /// NewMusicInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditMusicInfoDialog : Window
    {
        public EditMusicInfoDialog()
        {
            InitializeComponent();
        }

        public string Mark => TxtMark.Text.Trim();

        public string FilePath => TxtFilePath.Text.Trim();

        public double Volume => SliderVolume.Value;

        public bool IsRepeat => CboRepeat.SelectedValue.ToBoolean();

        public DialogMusicInfo MusicInfo { private set; get; }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                TxtFilePath.Text = openFile.FileName;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Mark.IsNullOrEmpty() && !FilePath.IsNullOrEmpty())
            {
                DialogResult = true;
                MusicInfo = new DialogMusicInfo(Mark)
                            {
                                FilePath = FilePath,
                                Volume = Volume,
                                IsRepeat = IsRepeat
                            };
                Close();
            }
        }
    }
}