// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Notification.Contracts;
using TLAuto.Notification.ServerHost.Common;
using TLAuto.Notification.ServerHost.IocService;
using TLAuto.Notification.ServerHost.ViewModels;
#endregion

namespace TLAuto.Notification.ServerHost
{
    public class TLNotificationService : ITLNotification
    {
        public WcfNotificationSourceService NotificationSourceService => (WcfNotificationSourceService)ViewModelLocator.Instance.NotificationSourceService;

        public async Task<bool> TestConnected()
        {
            return await NotificationSourceService.TestConnected();
        }

        public async Task<bool> AddAppStatus(string appKey, AppStatusType appStatusType)
        {
            return await NotificationSourceService.AddAppStatus(appKey, appStatusType);
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
            return await NotificationSourceService.StartApp(appKey, filePath, startArgs, inputBoardInfos, outputBoardInfos, musicInfos);
        }

        public async Task<bool> StopApp(string processName)
        {
            return await NotificationSourceService.StopApp(processName);
        }

        public async Task<bool> ShutDown()
        {
            await Task.Factory.StartNew(async () =>
                                        {
                                            await Task.Delay(5000);
                                            WinApi.DoExitWin(WinApi.EWX_SHUTDOWN);
                                        });
            return true;
        }

        public async Task<AppStatusType> GetAppStatus(string appKey)
        {
            return await NotificationSourceService.GetAppStatus(appKey);
        }
    }
}