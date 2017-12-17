// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using Microsoft.Win32;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.Music.Contracts;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Music
{
    [Description("播放指定音乐")]
    public class PlayMusicActionExcute : MusicActionExcute
    {
        private string _filePath;

        private bool _isRepeat;

        private MusicMarkManager _markManager;

        private double _volume = 0.5;

        public PlayMusicActionExcute()
        {
            InitBrowseCommand();
        }

        public string FilePath
        {
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
            get => _filePath;
        }

        public double Volume
        {
            set
            {
                _volume = value;
                RaisePropertyChanged();
            }
            get => _volume;
        }

        public bool IsRepeat
        {
            set
            {
                _isRepeat = value;
                RaisePropertyChanged();
            }
            get => _isRepeat;
        }

        public MusicMarkManager MarkManager { set => _markManager = value; get => _markManager ?? (_markManager = new MusicMarkManager()); }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var musicServiceAddress = ProjectHelper.GetMusicServiceAddress(MarkManager.SelectedMusicMark);
            if (musicServiceAddress.IsNullOrEmpty())
            {
                writeLogMsgAction("执行任务时出错，音乐服务地址为空。");
                return false;
            }
            var sendWcfCommand = new SendWcfCommand<ITLMusic>(musicServiceAddress, writeLogMsgAction);
            return await sendWcfCommand.SendAsync(async proxy => await proxy.PlayMusic(MarkManager.SelectedMusicMark, FilePath, Volume, IsRepeat));
        }

        #region Event MvvmBindings
        private void InitBrowseCommand()
        {
            BrowseCommand = new RelayCommand(() =>
                                             {
                                                 var openFile = new OpenFileDialog();
                                                 if (openFile.ShowDialog() == true)
                                                 {
                                                     FilePath = openFile.FileName;
                                                 }
                                             });
        }

        [XmlIgnore]
        public RelayCommand BrowseCommand { private set; get; }
        #endregion
    }
}