// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
using TLAuto.Video.App.Common;
using TLAuto.Video.Contracts;
using TLAuto.Video.Controls.AttachedPropertys;
using TLAuto.Wcf.Client;
using TLAuto.Wcf.Server;
using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Video.App.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly NetTcpWcfServerService _tlautoDeviceVideoService = new NetTcpWcfServerService(TimeSpan.FromMinutes(10), true);

        private bool _isOpen;

        private bool _isRepeat;

        private Uri _source;

        private string _title = "中控-视频";

        private VideoWindowType _videoWindowType = VideoWindowType.Default;

        private double _volume = 1;

        public MainViewModel()
        {
            _tlautoDeviceVideoService.Opened += VideoSourceService_Opened;
            _tlautoDeviceVideoService.Closed += VideoSourceService_Closed;
            _tlautoDeviceVideoService.Error += VideoSourceService_ErrorReceived;
            InitPreviewKeyDownCommand();
            InitLoadedCommand();
        }

        public string Title
        {
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
            get => _title;
        }

        public VideoWindowType VideoWindowType
        {
            set
            {
                _videoWindowType = value;
                RaisePropertyChanged();
            }
            get => _videoWindowType;
        }

        public Uri Source
        {
            set
            {
                _source = value;
                RaisePropertyChanged();
            }
            get => _source;
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

        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                RaisePropertyChanged();
            }
            get => _isOpen;
        }

        #region Methods
        private void UpdateTitle(string msg)
        {
            Title = $"中控-视频（提示：{msg}）";
        }

        private async void VideoSourceService_Opened(object sender, EventArgs e)
        {
            IsOpen = true;
            UpdateTitle("通信服务已经启动");
            var result = await SendCommand(async proxy => await proxy.TestConnected());
            if (result)
            {
                UpdateTitle("尝试调用接口成功。");
            }
        }

        private void VideoSourceService_Closed(object sender, WcfServerServiceMessageEventArgs e)
        {
            IsOpen = false;
        }

        private void VideoSourceService_ErrorReceived(object sender, WcfServerServiceErrorMessageEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { UpdateTitle($"开启通讯服务出现错误，原因：{e.Msg}"); });
        }

        private async Task<bool> StartMusicService()
        {
            if (IsOpen)
            {
                return false;
            }
            if (ConfigHelper.ServiceAddress.IsNullOrEmpty())
            {
                MessageBox.Show("服务地址参数为空。");
                return false;
            }
            return await _tlautoDeviceVideoService.StartWcfService(ConfigHelper.ServiceAddress, typeof(TLVideoService), typeof(ITLVideo));
        }

        private async Task<T> SendCommand<T>(Func<ITLVideo, Task<T>> func)
        {
            if (!IsOpen)
            {
                MessageBox.Show("通讯服务未启动。");
                return default(T);
            }
            var videoSendCommand = new NetTcpWcfClientService<ITLVideo>(ConfigHelper.ServiceAddress);
            var result = await videoSendCommand.SendAsync(func);
            return result;
        }
        #endregion

        #region Mvvm Event Bindings
        private void InitPreviewKeyDownCommand()
        {
            PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(async e =>
                                                                   {
                                                                       if (e.Key == Key.Escape)
                                                                       {
                                                                           if (VideoWindowType == VideoWindowType.FullScreen)
                                                                           {
                                                                               VideoWindowType = VideoWindowType.Default;
                                                                           }
                                                                           else
                                                                           {
                                                                               if (VideoWindowType == VideoWindowType.Default)
                                                                               {
                                                                                   Environment.Exit(1);
                                                                               }
                                                                           }
                                                                       }
                                                                       else
                                                                       {
                                                                           if (e.Key == Key.Enter)
                                                                           {
                                                                               if (VideoWindowType == VideoWindowType.FullScreen)
                                                                               {
                                                                                   VideoWindowType = VideoWindowType.Default;
                                                                               }
                                                                               else
                                                                               {
                                                                                   if (VideoWindowType == VideoWindowType.Default)
                                                                                   {
                                                                                       VideoWindowType = VideoWindowType.FullScreen;
                                                                                   }
                                                                               }
                                                                           }
                                                                       }
                                                                       //else if (e.Key == Key.A)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.SetPauseTimeEvent(TimeSpan.FromSeconds(2)));
                                                                       //}
                                                                       //else if (e.Key == Key.S)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.PlayVideo("D:/leaving.mp4", 1, true));
                                                                       //}
                                                                       //else if (e.Key == Key.Z)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.InvokerAction(VideoActionType.Play));
                                                                       //}
                                                                       //else if (e.Key == Key.X)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.InvokerAction(VideoActionType.Pause));
                                                                       //}
                                                                       //else if (e.Key == Key.C)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.InvokerAction(VideoActionType.Stop));
                                                                       //}
                                                                       //else if (e.Key == Key.V)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.ChangeFrame(TimeSpan.FromSeconds(3), VideoActionType.Pause));
                                                                       //}
                                                                       //else if (e.Key == Key.B)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.ChangeFrame(TimeSpan.FromSeconds(3), VideoActionType.Play));
                                                                       //}
                                                                       //else if (e.Key == Key.N)
                                                                       //{
                                                                       //    await SendCommand(async proxy => await proxy.ChangeFrame(TimeSpan.FromSeconds(3), VideoActionType.Stop));
                                                                       //}
                                                                   });
        }

        public RelayCommand<KeyEventArgs> PreviewKeyDownCommand { private set; get; }

        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(async () =>
                                             {
                                                 var result = await StartMusicService();
                                                 if (!result)
                                                 {
                                                     Environment.Exit(1);
                                                 }
                                                 else
                                                 {
                                                     IsOpen = true;
                                                     UpdateTitle("WCF服务启动成功");
                                                 }
                                             });
        }

        public RelayCommand LoadedCommand { private set; get; }
        #endregion
    }
}