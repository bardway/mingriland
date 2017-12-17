// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.Music.Contracts;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Music
{
    [Description("文本转语音")]
    public class TextToSpeakActionExcute : MusicActionExcute
    {
        private string _speakText;

        private int _volume = 50;

        public TextToSpeakActionExcute()
        {
            MarkManager = new MusicMarkManager();
        }

        public MusicMarkManager MarkManager { set; get; }

        public string SpeakText
        {
            set
            {
                _speakText = value;
                RaisePropertyChanged();
            }
            get => _speakText;
        }

        public int Volume
        {
            set
            {
                _volume = value;
                RaisePropertyChanged();
            }
            get => _volume;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var musicServiceAddress = ProjectHelper.GetMusicServiceAddress(MarkManager.SelectedMusicMark);
            if (musicServiceAddress.IsNullOrEmpty())
            {
                writeLogMsgAction("执行任务时出错，音乐服务地址为空。");
                return false;
            }
            var sendWcfCommand = new SendWcfCommand<ITLMusic>(musicServiceAddress, writeLogMsgAction);
            return await sendWcfCommand.SendAsync(async proxy => await proxy.SpeakFromText(SpeakText, Volume));
        }
    }
}