// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
using TLAuto.Notification.Contracts;
using TLAuto.Notification.ServerHost.Models;
#endregion

namespace TLAuto.Notification.ServerHost.ViewModels
{
    public class NotificationViewModel : ObservableObject
    {
        #region Methods
        private async Task<T> SendCommand<T>(Func<Task<T>> func)
        {
            if (!IsSendCommand)
            {
                IsSendCommand = true;
            }
            var result = await func();
            IsSendCommand = false;
            return result;
        }

        public async Task<bool> AddAppStatus(string appKey, AppStatusType appStatusType)
        {
            var result = await SendCommand(async () =>
                                           {
                                               await DispatcherHelper.RunAsync(() =>
                                                                               {
                                                                                   Messenger.Default.Send(new NotificationMessage(string.Format("{0} 正在添加App状态命令。", appKey)));
                                                                                   var notificationInfo = _notificationInfos.FirstOrDefault(s => s.AppKey == appKey);
                                                                                   if (notificationInfo == null)
                                                                                   {
                                                                                       var newNotificationInfo = new NotificationInfo(appKey)
                                                                                                                 {
                                                                                                                     AppStatusType = appStatusType
                                                                                                                 };
                                                                                       newNotificationInfo.Removed += NewNotificationInfo_Removed;
                                                                                       _notificationInfos.Add(newNotificationInfo);
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       notificationInfo.AppStatusType = appStatusType;
                                                                                   }
                                                                               });
                                               return true;
                                           });
            return result;
        }

        public async Task<AppStatusType> GetAppStatus(string appKey)
        {
            var result = await SendCommand(async () =>
                                           {
                                               var appStatusType = AppStatusType.Stop;
                                               await DispatcherHelper.RunAsync(() =>
                                                                               {
                                                                                   Messenger.Default.Send(new NotificationMessage(string.Format("{0} 正在获取App状态命令。", appKey)));
                                                                                   var notificationInfo = _notificationInfos.FirstOrDefault(s => s.AppKey == appKey);
                                                                                   if (notificationInfo != null)
                                                                                   {
                                                                                       appStatusType = notificationInfo.AppStatusType;
                                                                                   }
                                                                               });
                                               return appStatusType;
                                           });
            return result;
        }

        public async Task<bool> StartApp
        (
            string appKey,
            string filePath,
            string startArgs,
            IEnumerable<BoardNotificationInfo> inputBoardInfos,
            IEnumerable<BoardNotificationInfo> outputBoardInfos,
            IEnumerable<string> musicInfos)
        {
            var result = await SendCommand(async () =>
                                           {
                                               var appParamInfo = new AppParamInfo();
                                               foreach (var inputBoardParamInfo in inputBoardInfos)
                                               {
                                                   appParamInfo.InputBoardParamInfos.Add(new AppBoardParamInfo
                                                                                         {
                                                                                             DeviceNumber = inputBoardParamInfo.DeviceNumber,
                                                                                             Number = inputBoardParamInfo.Number,
                                                                                             ServiceAddress = inputBoardParamInfo.ServiceAddress,
                                                                                             PortName = inputBoardParamInfo.PortName
                                                                                         });
                                               }
                                               foreach (var outputBoardParamInfo in outputBoardInfos)
                                               {
                                                   appParamInfo.OutputBoardParamInfos.Add(new AppBoardParamInfo
                                                                                          {
                                                                                              DeviceNumber = outputBoardParamInfo.DeviceNumber,
                                                                                              Number = outputBoardParamInfo.Number,
                                                                                              ServiceAddress = outputBoardParamInfo.ServiceAddress,
                                                                                              PortName = outputBoardParamInfo.PortName
                                                                                          });
                                               }
                                               foreach (var musicParamInfo in musicInfos)
                                               {
                                                   appParamInfo.MusicServiceAddressList.Add(musicParamInfo);
                                               }
                                               try
                                               {
                                                   appParamInfo.ToXmlFile(Path.Combine(Path.GetDirectoryName(filePath), "AppParamInfo.xml"));
                                                   Messenger.Default.Send(new NotificationMessage(string.Format("{0} 正在启动 {0} App。", appKey)));
                                                   Process.Start(filePath, startArgs);
                                                   var isStartApp = false;
                                                   await Task.Factory.StartNew(() =>
                                                                               {
                                                                                   var fileName = Path.GetFileNameWithoutExtension(filePath);
                                                                                   var checkIndex = 5;
                                                                                   while (checkIndex != 0)
                                                                                   {
                                                                                       var processList = Process.GetProcessesByName(fileName).ToList();
                                                                                       if (processList.Count > 0)
                                                                                       {
                                                                                           isStartApp = true;
                                                                                           break;
                                                                                       }
                                                                                       Task.Delay(1000);
                                                                                       checkIndex--;
                                                                                   }
                                                                               });
                                                   return isStartApp;
                                               }
                                               catch (Exception ex)
                                               {
                                                   Messenger.Default.Send(new NotificationMessage(string.Format("{0} 启动APP出现异常，原因：{1}", appKey, ex.Message)));
                                                   return false;
                                               }
                                               return false;
                                           });
            return result;
        }

        private void NewNotificationInfo_Removed(object sender, EventArgs e)
        {
            var notificationInfo = (NotificationInfo)sender;
            notificationInfo.Removed -= NewNotificationInfo_Removed;
            _notificationInfos.Remove(notificationInfo);
        }
        #endregion

        #region Properties
        private bool _isSendCommand;

        public bool IsSendCommand
        {
            set
            {
                _isSendCommand = value;
                RaisePropertyChanged();
            }
            get => _isSendCommand;
        }

        private readonly ObservableCollection<NotificationInfo> _notificationInfos = new ObservableCollection<NotificationInfo>();

        public IEnumerable<NotificationInfo> NotificationInfos => _notificationInfos;
        #endregion
    }
}