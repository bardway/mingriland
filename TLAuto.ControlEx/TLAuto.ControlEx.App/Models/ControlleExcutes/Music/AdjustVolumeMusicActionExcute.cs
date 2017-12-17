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
    [Description("调整指定音量")]
    public class AdjustVolumeMusicActionExcute : MusicActionExcute
    {
        private bool _isAdjustVolumeAll;

        private double _volume = 0.5;

        public AdjustVolumeMusicActionExcute()
        {
            MarkManager = new MusicMarkManager();
        }

        public bool IsAdjustVolumeAll
        {
            set
            {
                _isAdjustVolumeAll = value;
                RaisePropertyChanged();
            }
            get => _isAdjustVolumeAll;
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

        public MusicMarkManager MarkManager { set; get; }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            if (IsAdjustVolumeAll)
            {
                var serviceAddressList = ProjectHelper.GetMusicAllServiceAddress();
                foreach (var serviceAddress in serviceAddressList)
                {
                    var sendWcfCommand = new SendWcfCommand<ITLMusic>(serviceAddress, writeLogMsgAction);
                    var result = await sendWcfCommand.SendAsync(async proxy => await proxy.AdjustAllVolumeMusic(Volume));
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
                var result = await sendWcfCommand.SendAsync(async proxy => await proxy.AdjustVolumeMusic(MarkManager.SelectedMusicMark, Volume));
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