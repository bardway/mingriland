// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
using TLAuto.Music.Contracts;
using TLAuto.Music.ServerHost.Common;
using TLAuto.Music.ServerHost.Dialog;
using TLAuto.Music.ServerHost.Dialog.Models;
using TLAuto.Music.ServerHost.IocService;
using TLAuto.Wcf.Client;
using TLAuto.Wcf.Server;
using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Music.ServerHost.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly NetTcpWcfServerService _tlautoDeviceMusicService = new NetTcpWcfServerService(TimeSpan.FromMinutes(10), true);

        #region Ctors
        public MainViewModel(IMusicSourceService musicSourceService)
        {
            musicSourceService.SetMusicUI(MusicVm);
            _tlautoDeviceMusicService.Opened += MusicSourceService_Opened;
            _tlautoDeviceMusicService.Closed += MusicSourceService_Closed;
            _tlautoDeviceMusicService.Error += MusicSourceService_ErrorReceived;
            InitEventMvvmBindings();
        }
        #endregion

        #region Methods
        private void InitEventMvvmBindings()
        {
            InitLoadedCommand();
            InitClosingCommand();
            InitStartServiceHostCommand();
            InitStopServiceHostCommand();
            InitSettingsServiceAddressCommand();
            InitPlayCommand();
            InitPauseCommand();
            InitPauseAllCommand();
            InitAdjustVolumeCommand();
            InitAdjustVolumeAllCommand();
            InitSpeakCommand();
        }

        private async void MusicSourceService_Opened(object sender, EventArgs e)
        {
            IsOpen = true;
            Messenger.Default.Send(new NotificationMessage("通信服务已经启动。"));
            var result = await ViewModelLocator.Instance.MusicSourceService.TestConnected();
            if (result)
            {
                Messenger.Default.Send(new NotificationMessage("尝试调用接口成功。"));
            }
        }

        private void MusicSourceService_Closed(object sender, WcfServerServiceMessageEventArgs e)
        {
            IsOpen = false;
        }

        private void MusicSourceService_ErrorReceived(object sender, WcfServerServiceErrorMessageEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { Messenger.Default.Send(new NotificationMessage("开启通讯服务出现错误，原因：" + e.Msg)); });
        }

        private void StartMusicService()
        {
            if (IsOpen)
            {
                return;
            }
            if (ConfigHelper.ServiceAddress.IsNullOrEmpty())
            {
                MessageBox.Show("服务地址参数为空。");
                return;
            }
            _tlautoDeviceMusicService.StartWcfService(ConfigHelper.ServiceAddress, typeof(TLMusicService), typeof(ITLMusic));
        }

        private void StopMusicService()
        {
            if (IsOpen)
            {
                _tlautoDeviceMusicService.StopWcfService();
            }
        }

        private async Task<T> SendCommand<T>(Func<ITLMusic, Task<T>> func)
        {
            if (!IsOpen)
            {
                MessageBox.Show("通讯服务未启动。");
                return default(T);
            }
            MusicVm.IsSendCommand = true;

            var musicSendCommand = new NetTcpWcfClientService<ITLMusic>(ConfigHelper.ServiceAddress);
            musicSendCommand.Error += (s, e) => { MusicVm.WriteMsg(e.Msg); };
            musicSendCommand.CommunicationError += (s, e) => { MusicVm.WriteMsg(e.Msg); };
            musicSendCommand.TimeoutError += (s, e) => { MusicVm.WriteMsg(e.Msg); };
            var result = await musicSendCommand.SendAsync(func);
            MusicVm.IsSendCommand = false;
            return result;
        }
        #endregion

        #region Properties
        private bool _isOpen;

        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                RaisePropertyChanged();
            }
            get => _isOpen;
        }

        public MusicViewModel MusicVm { get; } = new MusicViewModel();
        #endregion

        #region Event MvvmBindings
        private void InitLoadedCommand()
        {
            LoadedCommand = new RelayCommand(StartMusicService);
        }

        public RelayCommand LoadedCommand { private set; get; }

        private void InitClosingCommand()
        {
            ClosingCommand = new RelayCommand<CancelEventArgs>(e =>
                                                               {
                                                                   if (IsOpen)
                                                                   {
                                                                       if (MessageBox.Show("服务正在运行中，还是需要关闭吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                                                       {
                                                                           e.Cancel = true;
                                                                       }
                                                                       StopMusicService();
                                                                   }
                                                               });
        }

        public RelayCommand<CancelEventArgs> ClosingCommand { private set; get; }

        #region Menu Settings
        private void InitStartServiceHostCommand()
        {
            StartServiceHostCommand = new RelayCommand(StartMusicService);
        }

        public RelayCommand StartServiceHostCommand { private set; get; }

        private void InitStopServiceHostCommand()
        {
            StopServiceHostCommand = new RelayCommand(StopMusicService);
        }

        public RelayCommand StopServiceHostCommand { private set; get; }

        private void InitSettingsServiceAddressCommand()
        {
            SettingsServiceAddressCommand = new RelayCommand(() =>
                                                             {
                                                                 var sad = new SettingServiceAddressDialog(ConfigHelper.ServiceAddress);
                                                                 if (sad.ShowDialog() == true)
                                                                 {
                                                                     ConfigHelper.SaveConfig(sad.ServiceAddress);
                                                                 }
                                                             });
        }

        public RelayCommand SettingsServiceAddressCommand { private set; get; }
        #endregion

        #region Control
        private void InitPlayCommand()
        {
            PlayCommand = new RelayCommand(async () =>
                                           {
                                               var emd = new EditMusicInfoDialog();
                                               if (emd.ShowDialog() == true)
                                               {
                                                   var musicInfo = emd.MusicInfo;
                                                   var result = await SendCommand(async proxy => await proxy.PlayMusic(musicInfo.Mark, musicInfo.FilePath, musicInfo.Volume, musicInfo.IsRepeat));
                                                   Messenger.Default.Send(new NotificationMessage(result ? "播放命令调用成功" : "播放命令调用失败"));
                                               }
                                           });
        }

        public RelayCommand PlayCommand { private set; get; }

        private void InitPauseCommand()
        {
            PauseCommand = new RelayCommand(async () =>
                                            {
                                                var marks = await SendCommand(async proxy => await proxy.GetMusicMarks());
                                                var emd = new EditMusicMarkDialog(marks);
                                                if (emd.ShowDialog() == true)
                                                {
                                                    var mark = emd.Mark;
                                                    var musicInfo = new DialogMusicInfo(mark);
                                                    var result = await SendCommand(async proxy => await proxy.PauseMusic(musicInfo.Mark));
                                                    Messenger.Default.Send(new NotificationMessage(result ? "暂停命令调用成功" : "暂停命令调用失败"));
                                                }
                                            });
        }

        public RelayCommand PauseCommand { private set; get; }

        private void InitPauseAllCommand()
        {
            PauseAllCommand = new RelayCommand(async () =>
                                               {
                                                   var result = await SendCommand(async proxy => await proxy.PauseAllMusic());
                                                   Messenger.Default.Send(new NotificationMessage(result ? "全部暂停命令调用成功" : "全部暂停命令调用失败，没有可用的Mark。"));
                                               });
        }

        public RelayCommand PauseAllCommand { private set; get; }

        private void InitAdjustVolumeCommand()
        {
            AdjustVolumeCommand = new RelayCommand(async () =>
                                                   {
                                                       var marks = await SendCommand(async proxy => await proxy.GetMusicMarks());
                                                       var emd = new EditMusicVolumeDialog(false, marks);
                                                       if (emd.ShowDialog() == true)
                                                       {
                                                           var mark = emd.Mark;
                                                           var volume = emd.Volume;
                                                           var result = await SendCommand(async proxy => await proxy.AdjustVolumeMusic(mark, volume));
                                                           Messenger.Default.Send(new NotificationMessage(result ? "调整音量命令调用成功" : "调整音量命令调用失败"));
                                                       }
                                                   });
        }

        public RelayCommand AdjustVolumeCommand { private set; get; }

        private void InitAdjustVolumeAllCommand()
        {
            AdjustVolumeAllCommand = new RelayCommand(async () =>
                                                      {
                                                          var emd = new EditMusicVolumeDialog(true, null);
                                                          if (emd.ShowDialog() == true)
                                                          {
                                                              var volume = emd.Volume;
                                                              var result = await SendCommand(async proxy => await proxy.AdjustAllVolumeMusic(volume));
                                                              Messenger.Default.Send(new NotificationMessage(result ? "调整所有音量命令调用成功" : "调整所有音量命令调用失败，没有可用的Mark。"));
                                                          }
                                                      });
        }

        public RelayCommand AdjustVolumeAllCommand { private set; get; }

        private void InitSpeakCommand()
        {
            SpeakCommand = new RelayCommand(async () =>
                                            {
                                                var speakDialog = new SpeakTextDialog();
                                                if (speakDialog.ShowDialog() == true)
                                                {
                                                    var speakText = speakDialog.SpeakText;
                                                    var result = await SendCommand(async proxy => await proxy.SpeakFromText(speakText, 100));
                                                    Messenger.Default.Send(new NotificationMessage(result ? "文本转语音播放成功。" : "文本转语音播放失败。"));
                                                }
                                            });
        }

        public RelayCommand SpeakCommand { private set; get; }
        #endregion
        #endregion
    }
}