// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Controls;

using TLAuto.ControlEx.App.Models.ControlleExcutes;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Board;
using TLAuto.ControlEx.App.Models.ControlleExcutes.DMX;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Notification;
#endregion

namespace TLAuto.ControlEx.App.TemplateSelector
{
    public class ControllerExcuteDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BoardControllerExcuteDataTemplate { set; get; }

        public DataTemplate DelayControllerExcuteDataTemplate { set; get; }

        public DataTemplate ProjectorControllerExcuteDataTemplate { set; get; }

        public DataTemplate MusicControllerExcuteDataTemplate { set; get; }

        public DataTemplate NotificationControllerExcuteDataTemplate { set; get; }

        public DataTemplate DMXControllerExcuteDataTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SwitchBoardControllerExcute)
            {
                return BoardControllerExcuteDataTemplate;
            }
            if (item is RelayBoardControllerExcute)
            {
                return BoardControllerExcuteDataTemplate;
            }
            if (item is ProjectorControllerExcute)
            {
                return ProjectorControllerExcuteDataTemplate;
            }
            if (item is DelayControllerExcute)
            {
                return DelayControllerExcuteDataTemplate;
            }
            if (item is MusicControllerExcute)
            {
                return MusicControllerExcuteDataTemplate;
            }
            if (item is NotificationControllerExcute)
            {
                return NotificationControllerExcuteDataTemplate;
            }
            if (item is DMXControllerExcute)
            {
                return DMXControllerExcuteDataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}