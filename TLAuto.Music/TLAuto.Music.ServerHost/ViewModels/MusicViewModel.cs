// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.Music.Controls;
using TLAuto.Music.ServerHost.Models;
#endregion

namespace TLAuto.Music.ServerHost.ViewModels
{
    public class MusicViewModel : ObservableObject
    {
        private static readonly SpeechSynthesizer Synth = new SpeechSynthesizer();

        private readonly ObservableCollection<MusicInfo> _musicInfos = new ObservableCollection<MusicInfo>();

        private bool _isSendCommand;

        public MusicViewModel()
        {
            Synth.Volume = 100;
            Synth.Rate = 0;
        }

        public bool IsSendCommand
        {
            set
            {
                _isSendCommand = value;
                RaisePropertyChanged();
            }
            get => _isSendCommand;
        }

        public IEnumerable<MusicInfo> MusicInfos => _musicInfos;

        private async Task<T> SendCommand<T>(Func<Task<T>> func)
        {
            if (!IsSendCommand)
            {
                IsSendCommand = true;
            }
            var result = await func();
            IsSendCommand = false;
            return result;
        }

        public void WriteMsg(string msg)
        {
            Messenger.Default.Send(new NotificationMessage(msg));
        }

        public async Task<bool> PlayMusic(string mark, string filePath, double volume, bool isRepeat)
        {
            var result = await SendCommand(async () =>
                                           {
                                               await DispatcherHelper.RunAsync(() =>
                                                                               {
                                                                                   Messenger.Default.Send(new NotificationMessage(string.Format("{0} 正在调用播放命令。", mark)));
                                                                                   var musicInfo = _musicInfos.FirstOrDefault(s => s.Mark == mark);
                                                                                   if (musicInfo == null)
                                                                                   {
                                                                                       var newMusicInfo = new MusicInfo(mark)
                                                                                                          {
                                                                                                              FilePath = filePath,
                                                                                                              Volume = volume,
                                                                                                              IsRepeat = isRepeat
                                                                                                          };
                                                                                       newMusicInfo.Removed += NewMusicInfo_Removed;
                                                                                       _musicInfos.Add(newMusicInfo);
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       if (musicInfo.FilePath == filePath)
                                                                                       {
                                                                                           musicInfo.MusicStatusType = MusicStatusType.Stop;
                                                                                           musicInfo.MusicStatusType = MusicStatusType.Play;
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                           musicInfo.FilePath = filePath;
                                                                                       }
                                                                                       musicInfo.Volume = volume;
                                                                                       musicInfo.IsRepeat = isRepeat;
                                                                                   }
                                                                               });
                                               return true;
                                           });
            return result;
        }

        private void NewMusicInfo_Removed(object sender, EventArgs e)
        {
            var musicInfo = (MusicInfo)sender;
            musicInfo.Removed -= NewMusicInfo_Removed;
            _musicInfos.Remove(musicInfo);
        }

        public async Task<bool> PauseMusic(string mark)
        {
            var result = await SendCommand(async () =>
                                           {
                                               var currentResult = false;
                                               await DispatcherHelper.RunAsync(() =>
                                                                               {
                                                                                   WriteMsg(string.Format("{0} 正在调用暂停命令。", mark));
                                                                                   var musicInfo = _musicInfos.FirstOrDefault(s => s.Mark == mark);
                                                                                   if (musicInfo != null)
                                                                                   {
                                                                                       musicInfo.MusicStatusType = MusicStatusType.Pause;
                                                                                       currentResult = true;
                                                                                   }
                                                                               });
                                               return currentResult;
                                           });
            return result;
        }

        public async Task<bool> AdjustVolumeMusic(string mark, double volume)
        {
            var result = await SendCommand(async () =>
                                           {
                                               var currentResult = false;
                                               await DispatcherHelper.RunAsync(() =>
                                                                               {
                                                                                   WriteMsg(string.Format("{0} 正在调用调整音量命令。", mark));
                                                                                   var musicInfo = _musicInfos.FirstOrDefault(s => s.Mark == mark);
                                                                                   if (musicInfo != null)
                                                                                   {
                                                                                       musicInfo.Volume = volume;
                                                                                       currentResult = true;
                                                                                   }
                                                                               });
                                               return currentResult;
                                           });
            return result;
        }

        public async Task<bool> SpeakFromText(string text, int volume)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                                            {
                                                Synth.Volume = volume;
                                                Synth.Speak(text);
                                            });
                return true;
            }
            catch (Exception ex)
            {
                WriteMsg(ex.Message);
            }
            return false;
        }

        public async Task<IEnumerable<string>> GetMusicMarks()
        {
            var result = await SendCommand(async () =>
                                           {
                                               IEnumerable<string> musicMarks = new List<string>();
                                               await DispatcherHelper.RunAsync(() => { musicMarks = _musicInfos.Select(s => s.Mark).ToList(); });
                                               return musicMarks;
                                           });
            return result;
        }
    }
}