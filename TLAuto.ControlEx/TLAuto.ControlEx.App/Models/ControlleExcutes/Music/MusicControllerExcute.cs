// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Music
{
    [Description("音乐执行器")]
    public class MusicControllerExcute : ControllerExcute
    {
        public MusicControllerExcute()
        {
            InitAddMusicExcuteCommand();
            InitRemoveMusicExcuteCommand();
            InitUpMusicExcuteCommand();
            InitDownMusicExcuteCommand();
        }

        public ObservableCollection<MusicActionExcute> MusicActionExcutes { get; } = new ObservableCollection<MusicActionExcute>();

        protected override string GetDefaultDescription()
        {
            return "调用音乐总控";
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var list = new List<bool>();
            foreach (var musicActionExcute in MusicActionExcutes)
            {
                var result = await musicActionExcute.Excute(writeLogMsgAction);
                list.Add(result);
            }
            return list.All(s => s);
        }

        #region Event MvvmBindings
        private void InitAddMusicExcuteCommand()
        {
            AddMusicExcuteCommand = new RelayCommand(() =>
                                                     {
                                                         var nmc = new NewMusicControllerExcuteDialog();
                                                         if (nmc.ShowDialog() == true)
                                                         {
                                                             var musicType = nmc.SelectedMusicExcuteType;
                                                             MusicActionExcute musicActionExcute;
                                                             switch (musicType)
                                                             {
                                                                 case MusicControlType.Play:
                                                                     musicActionExcute = new PlayMusicActionExcute();
                                                                     break;
                                                                 case MusicControlType.Pause:
                                                                     musicActionExcute = new PauseMusicActionExcute();
                                                                     break;
                                                                 case MusicControlType.AdjustVolume:
                                                                     musicActionExcute = new AdjustVolumeMusicActionExcute();
                                                                     break;
                                                                 case MusicControlType.TextToSpeek:
                                                                     musicActionExcute = new TextToSpeakActionExcute();
                                                                     break;
                                                                 default:
                                                                     throw new ArgumentOutOfRangeException();
                                                             }
                                                             MusicActionExcutes.Add(musicActionExcute);
                                                         }
                                                     });
        }

        [XmlIgnore]
        public RelayCommand AddMusicExcuteCommand { private set; get; }

        private void InitRemoveMusicExcuteCommand()
        {
            RemoveMusicExcuteCommand = new RelayCommand(() =>
                                                        {
                                                            var removeItems = MusicActionExcutes.Where(s => s.IsChecked).ToList();
                                                            foreach (var item in removeItems)
                                                            {
                                                                MusicActionExcutes.Remove(item);
                                                            }
                                                        });
        }

        [XmlIgnore]
        public RelayCommand RemoveMusicExcuteCommand { private set; get; }

        private void InitUpMusicExcuteCommand()
        {
            UpMusicExcuteCommand = new RelayCommand(() =>
                                                    {
                                                        var excute = MusicActionExcutes.FirstOrDefault(s => s.IsChecked);
                                                        if (excute != null)
                                                        {
                                                            MusicActionExcutes.Up(excute);
                                                        }
                                                    });
        }

        [XmlIgnore]
        public RelayCommand UpMusicExcuteCommand { private set; get; }

        private void InitDownMusicExcuteCommand()
        {
            DownMusicExcuteCommand = new RelayCommand(() =>
                                                      {
                                                          var excute = MusicActionExcutes.FirstOrDefault(s => s.IsChecked);
                                                          if (excute != null)
                                                          {
                                                              MusicActionExcutes.Down(excute);
                                                          }
                                                      });
        }

        [XmlIgnore]
        public RelayCommand DownMusicExcuteCommand { private set; get; }
        #endregion
    }
}