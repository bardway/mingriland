// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Notification;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// EditAppMusicParamDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditAppMusicParamDialog : Window
    {
        private readonly ObservableCollection<MusicParamInfo> _musicParamInfos;

        public EditAppMusicParamDialog(ObservableCollection<MusicParamInfo> musicParamInfos)
        {
            InitializeComponent();
            _musicParamInfos = musicParamInfos;
            LstMusicParamInfos.ItemsSource = _musicParamInfos;
        }

        private void AddMusicServiceMark_Click(object sender, RoutedEventArgs e)
        {
            var sms = new SelectMarkServiceAddressDialog(ProjectHelper.Project.ItemXmlInfo.MusicGroup.MusicMarkMatchInfos.Select(s => s.Mark).ToList());
            if (sms.ShowDialog() == true)
            {
                var musicParamInfo = new MusicParamInfo
                                     {
                                         ServiceAddressMark = sms.SelectedMark
                                     };
                _musicParamInfos.Add(musicParamInfo);
            }
        }

        private void RemoveMusicServiceMark_Click(object sender, RoutedEventArgs e)
        {
            var removeInfo = _musicParamInfos.Where(s => s.IsChecked).ToList();
            foreach (var boardParamInfo in removeInfo)
            {
                _musicParamInfos.Remove(boardParamInfo);
            }
        }

        private void UpMusicServiceMark_Click(object sender, RoutedEventArgs e)
        {
            var musicParamInfo = _musicParamInfos.FirstOrDefault(s => s.IsChecked);
            if (musicParamInfo != null)
            {
                _musicParamInfos.Up(musicParamInfo);
            }
        }

        private void DownMusicServiceMark_Click(object sender, RoutedEventArgs e)
        {
            var musicParamInfo = _musicParamInfos.FirstOrDefault(s => s.IsChecked);
            if (musicParamInfo != null)
            {
                _musicParamInfos.Down(musicParamInfo);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}