// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using TLAuto.Notification.Contracts;
using TLAuto.Notification.ServerHost.Common;
using TLAuto.Notification.ServerHost.ViewModels;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Notification.ServerHost.IocService
{
    public sealed class WcfNotificationSourceService : INotificationSourceService
    {
        private static readonly ConcurrentDictionary<string, AppStatusType> AppStatusDic = new ConcurrentDictionary<string, AppStatusType>();
        private NotificationViewModel _notificationVm;

        public void SetNotificationUI(NotificationViewModel notificationVm)
        {
            _notificationVm = notificationVm;
        }

        public async Task<bool> TestConnected()
        {
            return await Task.Factory.StartNew(() => true);
        }

        public async Task<bool> AddAppStatus(string appKey, AppStatusType appStatusType)
        {
            var result = false;
            if (AppStatusDic.ContainsKey(appKey))
            {
                AppStatusType statusType;
                if (AppStatusDic.TryGetValue(appKey, out statusType))
                {
                    if (AppStatusDic.TryUpdate(appKey, appStatusType, statusType))
                    {
                        result = true;
                    }
                }
            }
            else
            {
                if (AppStatusDic.TryAdd(appKey, appStatusType))
                {
                    result = true;
                }
            }
            await _notificationVm.AddAppStatus(appKey, appStatusType);
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
            return await _notificationVm.StartApp(appKey, filePath, startArgs, inputBoardInfos, outputBoardInfos, musicInfos);
        }

        public async Task<bool> StopApp(string processName)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   var processList = Process.GetProcessesByName(processName);
                                                   foreach (var process in processList)
                                                   {
                                                       process.Kill();
                                                   }
                                                   return true;
                                               });
        }

        public Task<bool> ShutDown()
        {
            throw new NotImplementedException();
        }

        public async Task<AppStatusType> GetAppStatus(string appKey)
        {
            return await _notificationVm.GetAppStatus(appKey);
        }

        public async Task<bool> Connected()
        {
            var notificationSendCommand = new NetTcpWcfClientService<ITLNotification>(ConfigHelper.ServiceAddress);
            return await notificationSendCommand.SendAsync(async proxy => await proxy.TestConnected());
        }
    }
}