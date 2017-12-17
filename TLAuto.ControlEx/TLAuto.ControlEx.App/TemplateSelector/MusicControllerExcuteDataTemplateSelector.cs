// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Controls;

using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
#endregion

namespace TLAuto.ControlEx.App.TemplateSelector
{
    public class MusicControllerExcuteDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PlayDataTemplate { set; get; }

        public DataTemplate PauseDataTemplate { set; get; }

        public DataTemplate AdjustVolumeDataTemplate { set; get; }

        public DataTemplate TextToSpeakDataTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PlayMusicActionExcute)
            {
                return PlayDataTemplate;
            }
            if (item is PauseMusicActionExcute)
            {
                return PauseDataTemplate;
            }
            if (item is AdjustVolumeMusicActionExcute)
            {
                return AdjustVolumeDataTemplate;
            }
            if (item is TextToSpeakActionExcute)
            {
                return TextToSpeakDataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}