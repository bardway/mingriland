// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Video.App.ViewModels;
using TLAuto.Video.App.Views;
using TLAuto.Video.Contracts;
#endregion

namespace TLAuto.Video.App
{
    public class TLVideoService : ITLVideo
    {
        public async Task<bool> TestConnected()
        {
            return await Task.Factory.StartNew(() => true);
        }

        public async Task<bool> PlayVideo(string videoPath, double volume, bool isRepeat)
        {
            await DispatcherHelper.UIDispatcher.InvokeAsync(() =>
                                                            {
                                                                var mainVm = ViewModelLocator.Instance.MainViewModel;
                                                                mainVm.Source = new Uri(videoPath);
                                                                mainVm.Volume = volume;
                                                                mainVm.IsRepeat = isRepeat;
                                                            });
            return true;
        }

        public async Task<bool> InvokerAction(VideoActionType videoActionType)
        {
            await DispatcherHelper.UIDispatcher.InvokeAsync(() =>
                                                            {
                                                                var mediaElement = ((MainView)Application.Current.MainWindow).Vc;
                                                                switch (videoActionType)
                                                                {
                                                                    case VideoActionType.Play:
                                                                        mediaElement.Play();
                                                                        break;
                                                                    case VideoActionType.Pause:
                                                                        mediaElement.Pause();
                                                                        break;
                                                                    case VideoActionType.Stop:
                                                                        mediaElement.Stop();
                                                                        break;
                                                                    default:
                                                                        throw new ArgumentOutOfRangeException(nameof(videoActionType), videoActionType, null);
                                                                }
                                                            });
            return true;
        }

        public async Task<bool> SetPauseTimeEvent(TimeSpan time)
        {
            await DispatcherHelper.UIDispatcher.InvokeAsync(() =>
                                                            {
                                                                var mediaElement = ((MainView)Application.Current.MainWindow).Vc;
                                                                mediaElement.SetPauseTimeEvent(time);
                                                            });
            return true;
        }

        public async Task<bool> ChangeFrame(TimeSpan time, VideoActionType afterVideoActionType)
        {
            await DispatcherHelper.UIDispatcher.InvokeAsync(() =>
                                                            {
                                                                var mediaElement = ((MainView)Application.Current.MainWindow).Vc;
                                                                mediaElement.ChangeFrame(time);
                                                                switch (afterVideoActionType)
                                                                {
                                                                    case VideoActionType.Play:
                                                                        mediaElement.Play();
                                                                        break;
                                                                    case VideoActionType.Pause:
                                                                        mediaElement.Play();
                                                                        mediaElement.Pause();
                                                                        break;
                                                                    case VideoActionType.Stop:
                                                                        mediaElement.Play();
                                                                        mediaElement.Stop();
                                                                        break;
                                                                    default:
                                                                        throw new ArgumentOutOfRangeException(nameof(afterVideoActionType), afterVideoActionType, null);
                                                                }
                                                            });
            return true;
        }
    }
}