// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class MusicMarkMatchInfo : MarkMatchInfo
    {
        public MusicMarkMatchInfo()
        {
            InitUpdateMusicVolumeCommand();
        }

        [XmlIgnore]
        public RelayCommand UpdateMusicVolumeCommand { private set; get; }

        private void InitUpdateMusicVolumeCommand()
        {
            UpdateMusicVolumeCommand = new RelayCommand(() =>
                                                        {
                                                            var dialog = new EditMusicVolumeDialog();
                                                            if (dialog.ShowDialog() == true)
                                                            {
                                                                var musicControllerExcutes = ProjectHelper.GetMusicControllerExcutes();
                                                                foreach (var musicControllerExcute in musicControllerExcutes)
                                                                {
                                                                    List<PlayMusicActionExcute> playMusicActionExcutes;
                                                                    List<TextToSpeakActionExcute> textToSpeakActionExcutes;
                                                                    switch (dialog.SelectedMusicControlType)
                                                                    {
                                                                        case MusicControlType.Play:
                                                                            playMusicActionExcutes = musicControllerExcute.MusicActionExcutes.OfType<PlayMusicActionExcute>().Where(s => s.MarkManager.SelectedMusicMark == Mark).ToList();
                                                                            foreach (var playMusicActionExcute in playMusicActionExcutes)
                                                                            {
                                                                                playMusicActionExcute.Volume = dialog.Volume;
                                                                            }
                                                                            ProjectHelper.SaveAll(ProjectHelper.Project);
                                                                            break;
                                                                        case MusicControlType.TextToSpeek:
                                                                            textToSpeakActionExcutes = musicControllerExcute.MusicActionExcutes.OfType<TextToSpeakActionExcute>().Where(s => s.MarkManager.SelectedMusicMark == Mark).ToList();
                                                                            foreach (var textToSpeakMusicActionExcute in textToSpeakActionExcutes)
                                                                            {
                                                                                textToSpeakMusicActionExcute.Volume = (int)(dialog.Volume * 100);
                                                                            }
                                                                            ProjectHelper.SaveAll(ProjectHelper.Project);
                                                                            break;
                                                                        default:
                                                                            throw new ArgumentException();
                                                                    }
                                                                }
                                                            }
                                                        });
        }
    }
}