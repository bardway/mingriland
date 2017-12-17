// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.Notification.Contracts;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Notification
{
    [Description("App通知执行器")]
    public class NotificationControllerExcute : ControllerExcute
    {
        private string _breakProcessName;

        private string _filePath;
        private volatile bool _isBreak;

        private bool _isCheckedForSetAppStatus;
        private volatile bool _isStop;

        private string _processName;

        private AppStatusType _selectedAppStatusType;

        private NotificationCommandType _selectedNotificationCommandType;

        private string _selectedNotificationMark;

        private string _startArgs = "";

        public NotificationControllerExcute()
        {
            InitInputBoardParamCommand();
            InitOutputBoardParamCommand();
            InitMusicParamCommand();
            InitTestCommand();
            InitTestStartCommand();
            InitNotificationCommandTypes();
            InitAppStatusTypes();
        }

        public ObservableCollection<BoardParamInfo> InputBoardParamInfos { get; } = new ObservableCollection<BoardParamInfo>();

        public ObservableCollection<BoardParamInfo> OutputBoardParamInfos { get; } = new ObservableCollection<BoardParamInfo>();

        public ObservableCollection<MusicParamInfo> MusicParamInfo { get; } = new ObservableCollection<MusicParamInfo>();

        [XmlIgnore]
        public ObservableCollection<MarkMatchInfo> NotificationMarks => ProjectHelper.Project.ItemXmlInfo.NotificationGroup.MarkMatchInfos;

        [XmlIgnore]
        public List<NotificationCommandType> NotificationCommandTypes { get; } = new List<NotificationCommandType>();

        public NotificationCommandType SelectedNotificationCommandType
        {
            set
            {
                _selectedNotificationCommandType = value;
                RaisePropertyChanged();
            }
            get => _selectedNotificationCommandType;
        }

        [XmlIgnore]
        public List<AppStatusType> AppStatusTypes { get; } = new List<AppStatusType>();

        public bool IsCheckedForSetAppStatus
        {
            set
            {
                _isCheckedForSetAppStatus = value;
                RaisePropertyChanged();
            }
            get => _isCheckedForSetAppStatus;
        }

        public AppStatusType SelectedAppStatusType
        {
            set
            {
                _selectedAppStatusType = value;
                RaisePropertyChanged();
            }
            get => _selectedAppStatusType;
        }

        public string SelectedNotificationMark
        {
            set
            {
                _selectedNotificationMark = value;
                RaisePropertyChanged();
            }
            get => _selectedNotificationMark;
        }

        public string FilePath
        {
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
            get => _filePath;
        }

        public string StartArgs
        {
            set
            {
                _startArgs = value;
                RaisePropertyChanged();
            }
            get => _startArgs;
        }

        public string ProcessName
        {
            set
            {
                _processName = value;
                RaisePropertyChanged();
            }
            get => _processName;
        }

        public string BreakProcessName
        {
            set
            {
                _breakProcessName = value;
                RaisePropertyChanged();
            }
            get => _breakProcessName;
        }

        private void InitNotificationCommandTypes()
        {
            var names = Enum.GetNames(typeof(NotificationCommandType));
            foreach (var name in names)
            {
                var notificationCommandType = (NotificationCommandType)Enum.Parse(typeof(NotificationCommandType), name);
                NotificationCommandTypes.Add(notificationCommandType);
            }
            SelectedNotificationCommandType = NotificationCommandType.Status;
        }

        private void InitAppStatusTypes()
        {
            var names = Enum.GetNames(typeof(AppStatusType));
            foreach (var name in names)
            {
                var appStatusType = (AppStatusType)Enum.Parse(typeof(AppStatusType), name);
                AppStatusTypes.Add(appStatusType);
            }
            SelectedAppStatusType = AppStatusType.Start;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            _isStop = false;
            _isBreak = false;
            var serviceAddress = ProjectHelper.GetNotificationServiceAddress(SelectedNotificationMark);
            switch (SelectedNotificationCommandType)
            {
                case NotificationCommandType.Status:
                    return await RunStatusExcute(serviceAddress, writeLogMsgAction);
                case NotificationCommandType.StartApp:
                    return await RunStartAppExcute(serviceAddress, writeLogMsgAction);
                case NotificationCommandType.StopApp:
                    return await RunStopAppExcute(serviceAddress, writeLogMsgAction);
                case NotificationCommandType.ShutDown:
                    return await RunShutDownExcute(serviceAddress, writeLogMsgAction);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<bool> RunStatusExcute(string serviceAddress, Action<string> writeLogMsgAction)
        {
            if (IsCheckedForSetAppStatus)
            {
                var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
                var result = await sendWcfCommand.SendAsync(async proxy => await proxy.AddAppStatus(SelectedNotificationMark, SelectedAppStatusType));
                return result;
            }
            else
            {
                var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
                do
                {
                    var result = await sendWcfCommand.SendAsync(async proxy => await proxy.GetAppStatus(SelectedNotificationMark));
                    if (result == SelectedAppStatusType)
                    {
                        return true;
                    }
                    var delayIndex = 5;
                    while ((delayIndex != 0) && !_isStop)
                    {
                        await Task.Delay(1000);
                        delayIndex--;
                    }
                }
                while (!_isStop && !_isBreak);
                writeLogMsgAction(string.Format("检查APP：{0} 状态失败。", SelectedNotificationMark));
                if (_isBreak)
                {
                    return true;
                }
                return false;
            }
        }

        private async Task<bool> RunStartAppExcute(string serviceAddress, Action<string> writeLogMsgAction)
        {
            var selectedDiffType = ProjectHelper.Project.ItemXmlInfo.SelectedDiffType;
            var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
            var inputBoardInfos = InputBoardParamInfos.Select(s => new BoardNotificationInfo
                                                                   {
                                                                       DeviceNumber = s.DeviceNumber,
                                                                       Number = s.Number,
                                                                       ServiceAddress = ProjectHelper.GetBoardServiceAddress(s.DeviceNumber, BoardType.InputA),
                                                                       PortName = s.PortName
                                                                   }).ToList();
            var outputBoardInfos = OutputBoardParamInfos.Select(s => new BoardNotificationInfo
                                                                     {
                                                                         DeviceNumber = s.DeviceNumber,
                                                                         Number = s.Number,
                                                                         ServiceAddress = ProjectHelper.GetBoardServiceAddress(s.DeviceNumber, BoardType.OutputA),
                                                                         PortName = s.PortName
                                                                     }).ToList();
            var musicInfos = MusicParamInfo.Select(s => ProjectHelper.GetMusicServiceAddress(s.ServiceAddressMark)).ToList();
            return await sendWcfCommand.SendAsync(async proxy => await proxy.StartApp(SelectedNotificationMark, FilePath, StartArgs + (!StartArgs.Trim().IsNullOrEmpty() ? " " + selectedDiffType : selectedDiffType.ToString()), inputBoardInfos, outputBoardInfos, musicInfos));
        }

        private async Task<bool> RunStopAppExcute(string serviceAddress, Action<string> writeLogMsgAction)
        {
            var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
            return await sendWcfCommand.SendAsync(async proxy => await proxy.StopApp(ProcessName));
        }

        private async Task<bool> RunShutDownExcute(string serviceAddress, Action<string> writeLogMsgAction)
        {
            var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
            await sendWcfCommand.SendAsync(async proxy => await proxy.ShutDown());
            return true;
        }

        public override void StopExcute(Action<string> writeLogMsgAction)
        {
            _isStop = true;
        }

        public override async void BreakExcute(Action<string> writeLogMsgAction)
        {
            var serviceAddress = ProjectHelper.GetNotificationServiceAddress(SelectedNotificationMark);
            if (!BreakProcessName.IsNullOrEmpty())
            {
                var sendWcfCommand = new SendWcfCommand<ITLNotification>(serviceAddress, writeLogMsgAction);
                var result = await sendWcfCommand.SendAsync(async proxy => await proxy.StopApp(BreakProcessName));
                if (result)
                {
                    var result2 = await sendWcfCommand.SendAsync(async proxy => await proxy.AddAppStatus(SelectedNotificationMark, AppStatusType.Stop));
                    if (result2)
                    {
                        _isBreak = true;
                    }
                    else
                    {
                        MessageBox.Show("跳过失败，原因：" + "有可能通知Key没有选对。");
                    }
                }
                else
                {
                    MessageBox.Show("跳过失败，原因：" + "有可能进程名不对。");
                }
            }
        }

        #region Event MvvmBindings
        private void InitInputBoardParamCommand()
        {
            InputBoardParamCommand = new RelayCommand(() =>
                                                      {
                                                          var eap = new EditAppBoardParamDialog(BoardType.InputA, InputBoardParamInfos);
                                                          eap.ShowDialog();
                                                      });
        }

        [XmlIgnore]
        public RelayCommand InputBoardParamCommand { private set; get; }

        private void InitOutputBoardParamCommand()
        {
            OutputBoardParamCommand = new RelayCommand(() =>
                                                       {
                                                           var eap = new EditAppBoardParamDialog(BoardType.OutputA, OutputBoardParamInfos);
                                                           eap.ShowDialog();
                                                       });
        }

        [XmlIgnore]
        public RelayCommand OutputBoardParamCommand { private set; get; }

        private void InitMusicParamCommand()
        {
            MusicParamCommand = new RelayCommand(() =>
                                                 {
                                                     var emp = new EditAppMusicParamDialog(MusicParamInfo);
                                                     emp.ShowDialog();
                                                 });
        }

        [XmlIgnore]
        public RelayCommand MusicParamCommand { private set; get; }

        private void InitTestCommand()
        {
            TestCommand = new RelayCommand(() =>
                                           {
                                               var appParamInfo = BuildingAppXml();
                                               appParamInfo.ToXmlFile(Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "AppParamInfo.xml"));
                                           });
        }

        private AppParamInfo BuildingAppXml()
        {
            var projectItemXmlInfo = ProjectHelper.Project.ItemXmlInfo;
            var appParamInfo = new AppParamInfo();
            foreach (var inputBoardParamInfo in InputBoardParamInfos)
            {
                var serviceInfo =
                    projectItemXmlInfo.BoardServiceAddressInfos.FirstOrDefault(
                                                                               s => s.Mark == inputBoardParamInfo.ServiceAddressMark);
                if (serviceInfo != null)
                {
                    var serviceAddress = serviceInfo.ServiceAddress;
                    appParamInfo.InputBoardParamInfos.Add(new AppBoardParamInfo
                                                          {
                                                              DeviceNumber = inputBoardParamInfo.DeviceNumber,
                                                              Number = inputBoardParamInfo.Number,
                                                              ServiceAddress = serviceAddress,
                                                              PortName = inputBoardParamInfo.PortName
                                                          });
                }
            }
            foreach (var outputBoardParamInfo in OutputBoardParamInfos)
            {
                var serviceInfo =
                    projectItemXmlInfo.BoardServiceAddressInfos.FirstOrDefault(
                                                                               s => s.Mark == outputBoardParamInfo.ServiceAddressMark);
                if (serviceInfo != null)
                {
                    var serviceAddress = serviceInfo.ServiceAddress;
                    appParamInfo.OutputBoardParamInfos.Add(new AppBoardParamInfo
                                                           {
                                                               DeviceNumber = outputBoardParamInfo.DeviceNumber,
                                                               Number = outputBoardParamInfo.Number,
                                                               ServiceAddress = serviceAddress,
                                                               PortName = outputBoardParamInfo.PortName
                                                           });
                }
            }
            foreach (var musicParamInfo in MusicParamInfo)
            {
                var musicMarkInfo =
                    projectItemXmlInfo.MusicGroup.MusicMarkMatchInfos.FirstOrDefault(
                                                                                     s => s.Mark == musicParamInfo.ServiceAddressMark);
                if (musicMarkInfo != null)
                {
                    var serviceInfo =
                        projectItemXmlInfo.MusicGroup.MusicServiceAddressInfos.FirstOrDefault(
                                                                                              s => s.Mark == musicMarkInfo.ServiceAddressMark);
                    if (serviceInfo != null)
                    {
                        var serviceAddress = serviceInfo.ServiceAddress;
                        appParamInfo.MusicServiceAddressList.Add(serviceAddress);
                    }
                }
            }
            return appParamInfo;
        }

        [XmlIgnore]
        public RelayCommand TestCommand { private set; get; }

        private void InitTestStartCommand()
        {
            TestStartCommand = new RelayCommand(() =>
                                                {
                                                    var appParamInfo = BuildingAppXml();
                                                    var dir = Path.GetDirectoryName(FilePath);
                                                    appParamInfo.ToXmlFile(Path.Combine(dir, "AppParamInfo.xml"));
                                                    var selectedDiffType = ProjectHelper.Project.ItemXmlInfo.SelectedDiffType;
                                                    Process.Start(FilePath, StartArgs + (!StartArgs.Trim().IsNullOrEmpty() ? " " + selectedDiffType : selectedDiffType.ToString()));
                                                });
        }

        [XmlIgnore]
        public RelayCommand TestStartCommand { private set; get; }
        #endregion
    }
}