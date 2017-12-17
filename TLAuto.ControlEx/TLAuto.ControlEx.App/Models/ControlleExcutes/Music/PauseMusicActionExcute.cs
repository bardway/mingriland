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
    [Description("暂停指定音乐")]
    public class PauseMusicActionExcute : MusicActionExcute
    {
        private bool _isPauseAll;

        public PauseMusicActionExcute()
        {
            MarkManager = new MusicMarkManager();
        }

        public bool IsPauseAll
        {
            set
            {
                _isPauseAll = value;
                RaisePropertyChanged();
            }
            get => _isPauseAll;
        }

        public MusicMarkManager MarkManager { set; get; }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            if (IsPauseAll)
            {
                var serviceAddressList = ProjectHelper.GetMusicAllServiceAddress();
                foreach (var serviceAddress in serviceAddressList)
                {
                    var sendWcfCommand = new SendWcfCommand<ITLMusic>(serviceAddress, writeLogMsgAction);
                    var result = await sendWcfCommand.SendAsync(async proxy => await proxy.PauseAllMusic());
                    if (!result)
                    {
                        writeLogMsgAction("执行任务失败，原因可能是没有可操作的音乐标识符。");
                    }
                    return true;
                }
            }
            else
            {
                var musicServiceAddress = ProjectHelper.GetMusicServiceAddress(MarkManager.SelectedMusicMark);
                if (musicServiceAddress.IsNullOrEmpty())
                {
                    writeLogMsgAction("执行任务时出错，音乐服务地址为空。");
                    return false;
                }
                var sendWcfCommand = new SendWcfCommand<ITLMusic>(musicServiceAddress, writeLogMsgAction);
                var result = await sendWcfCommand.SendAsync(async proxy => await proxy.PauseMusic(MarkManager.SelectedMusicMark));
                if (!result)
                {
                    writeLogMsgAction("执行任务失败，原因可能是没有可操作的音乐标识符。");
                }
                return true;
            }
            return false;
        }
    }
}