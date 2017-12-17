// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Events;
using TLAuto.Music.Contracts;
using TLAuto.Notification.Contracts;
using TLAuto.Video.Contracts;
using TLAuto.Wcf.Client;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Machine.Plugins.Core
{
    public static class SendWcfCommandPluginsHelper
    {
        //public static async Task<bool> InvokerPlayVideo(string videoKey, string videoPath, string startArgs, string serviceAddress)
        //{
        //    var notificationService = new NetTcpWcfClientService<ITLNotification>(serviceAddress);
        //    RegWcfEvents(notificationService);
        //    var result = await notificationService.SendAsync(async proxy => await proxy.StartApp(videoKey, videoPath, startArgs, new List<BoardNotificationInfo>(), new List<BoardNotificationInfo>(), new List<string>()));
        //    UnregWcfEvents(notificationService);
        //    return result;
        //}

        //public static async Task<bool> InvokerStopVideo(string serviceAddress)
        //{
        //    var notificationService = new NetTcpWcfClientService<ITLNotification>(serviceAddress);
        //    RegWcfEvents(notificationService);
        //    var result = await notificationService.SendAsync(async proxy => await proxy.StopApp("TLAuto.Video.App"));
        //    UnregWcfEvents(notificationService);
        //    return result;
        //}

        public static async Task<bool> InvokerMusic(string musicKey, string filePath, double volume, bool isRepeat, string serviceAddress)
        {
            var notificationService = new NetTcpWcfClientService<ITLMusic>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.PlayMusic(musicKey, filePath, volume, isRepeat));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> PlayMusic0(string key, string fileName, string musicSericeAddress, string musicKey = null, double volume = 1, bool isRepeat = false)
        {
            var musicPathBase = CommonConfigHelper.MusicBasePath;
            musicPathBase = @"C:\Program Files\StartGateServer\TLAuto.Machine\" + key + @"\MachinePlugins\Music";
            return await InvokerMusic(key + (musicKey.IsNullOrEmpty() ? "" : musicKey), Path.Combine(musicPathBase, fileName), volume, isRepeat, musicSericeAddress);
        }

        public static async Task<bool> InvokerPauseMusic(string musicKey, string serviceAddress)
        {
            var notificationService = new NetTcpWcfClientService<ITLMusic>(serviceAddress);
            return await notificationService.SendAsync(async proxy => await proxy.PauseMusic(musicKey));
        }

        public static async Task<bool> InvokerTextToMusic(string text, string serviceAddress, int volume = 100)
        {
            var notificationService = new NetTcpWcfClientService<ITLMusic>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.SpeakFromText(text, volume));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> CheckButtonPressAsyncTask(List<MachineButtonItem> allCheckButtonItems, List<MachineButtonItem> raiseButtonItems, int queryTime)
        {
            var task = new CheckButtonPressAndControlAsyncTask(allCheckButtonItems, raiseButtonItems, queryTime);
            return await task.InvokeAsync();
        }

        public static async Task<MachineButtonItem> NotificationButtonPressAsyncTask(List<MachineButtonItem> checkButtonitems, int queryTime)
        {
            var task = new NotificationButtonPressAsyncTask(checkButtonitems, queryTime);
            return await task.InvokeAsync();
        }

        public static async Task<bool> InvokerQueryDiaitalSwitchWithAutoUpload(MachineButtonItem boardItem, int queryTime)
        {
            var boardSendCommand = new NetTcpWcfClientService<ITLAutoDevice>(boardItem.ServiceAddress, TimeSpan.FromMilliseconds(queryTime));
            var result = await boardSendCommand.SendAsync(async proxy =>
                                                          {
                                                              var controlInfo = new ControlInfo {ServiceKey = CommonConfigHelper.PLCServiceKey};
                                                              var serviceData = new PLCControlServiceData
                                                                                {
                                                                                    ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                    DeviceNumber = boardItem.DeviceNumber,
                                                                                    Number = new[] {boardItem.Number},
                                                                                    PortSignName = boardItem.SignName,
                                                                                    QueryTimeForAutoUpload = queryTime
                                                                                };
                                                              controlInfo.Data = serviceData.ToBytes();
                                                              var resultInfo = await proxy.ControlDevice(controlInfo);
                                                              if (!resultInfo.IsError && (resultInfo.Data != null))
                                                              {
                                                                  var switchItems = resultInfo.Data.ToObject<IEnumerable<SwitchItem>>().ToList();
                                                                  var switchItem = switchItems.Find(s => (s != null) && (s.SwitchNumber == boardItem.Number) && s.SwitchStatus.ToInt32().ToBoolean());
                                                                  if (switchItem != null)
                                                                  {
                                                                      return true;
                                                                  }
                                                              }
                                                              return false;
                                                          });
            return result;
        }

        public static async Task<bool> InvokerControlRelay(MachineRelayItem relayItem, bool isNo)
        {
            var notificationService = new NetTcpWcfClientService<ITLAutoDevice>(relayItem.ServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy =>
                                                             {
                                                                 var controlInfo = new ControlInfo {ServiceKey = CommonConfigHelper.PLCServiceKey};
                                                                 var serviceData = new PLCControlServiceData
                                                                                   {
                                                                                       ControlPLCType = ControlPLCType.ControlRelay,
                                                                                       DeviceNumber = relayItem.DeviceNumber,
                                                                                       Number = new[] {relayItem.Number},
                                                                                       PortSignName = relayItem.SignName,
                                                                                       RelayStatus = isNo
                                                                                   };
                                                                 controlInfo.Data = serviceData.ToBytes();
                                                                 var resultInfo = await proxy.ControlDevice(controlInfo);
                                                                 if (!resultInfo.IsError && (resultInfo.Data != null))
                                                                 {
                                                                     return true;
                                                                 }
                                                                 return false;
                                                             });
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> InvokerNotificationForStart(string notificationKey)
        {
            var notificationService = new NetTcpWcfClientService<ITLNotification>(CommonConfigHelper.NotificationServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.AddAppStatus(notificationKey, AppStatusType.Start));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> InvokerNotificationForStop(string notificationKey)
        {
            var notificationService = new NetTcpWcfClientService<ITLNotification>(CommonConfigHelper.NotificationServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.AddAppStatus(notificationKey, AppStatusType.Stop));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<AppStatusType> GetNotificationStatus(string notificationKey, string serviceAddress = null)
        {
            var notificationService = new NetTcpWcfClientService<ITLNotification>(serviceAddress ?? CommonConfigHelper.NotificationServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.GetAppStatus(notificationKey));
            UnregWcfEvents(notificationService);
            return result;
        }

        private static void RegWcfEvents<T>(IWcfClientService<T> wcfClientService)
        {
            wcfClientService.CommunicationError += WcfClientService_CommunicationError;
            wcfClientService.Error += WcfClientService_Error;
            wcfClientService.TimeoutError += WcfClientService_TimeoutError;
        }

        private static void UnregWcfEvents<T>(IWcfClientService<T> wcfClientService)
        {
            wcfClientService.CommunicationError -= WcfClientService_CommunicationError;
            wcfClientService.Error -= WcfClientService_Error;
            wcfClientService.TimeoutError -= WcfClientService_TimeoutError;
        }

        private static void WcfClientService_TimeoutError(object sender, WcfClientServiceErrorEventArgs e)
        {
            OnWcfNotification(new NotificationEventArgs(e.Msg));
        }

        private static void WcfClientService_Error(object sender, WcfClientServiceErrorEventArgs e)
        {
            OnWcfNotification(new NotificationEventArgs(e.Msg));
        }

        private static void WcfClientService_CommunicationError(object sender, WcfClientServiceErrorEventArgs e)
        {
            OnWcfNotification(new NotificationEventArgs(e.Msg));
        }

        public static event EventHandler<NotificationEventArgs> WcfNotification;

        private static void OnWcfNotification(NotificationEventArgs e)
        {
            WcfNotification?.Invoke(null, e);
        }

        #region Video
        public static async Task<bool> PlayVideo(string key, string fileName, double volume, bool isRepeat, string serviceAddress)
        {
            var filePathBase = @"C:\Program Files\StartGateServer\TLAuto.Machine\" + key + @"\MachinePlugins\Video";
            var notificationService = new NetTcpWcfClientService<ITLVideo>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.PlayVideo(Path.Combine(filePathBase, fileName), volume, isRepeat));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> InvokerAction(VideoActionType videoActionType, string serviceAddress)
        {
            var notificationService = new NetTcpWcfClientService<ITLVideo>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.InvokerAction(videoActionType));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> SetPauseTimeEvent(TimeSpan time, string serviceAddress)
        {
            var notificationService = new NetTcpWcfClientService<ITLVideo>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.SetPauseTimeEvent(time));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> ChangeFrame(TimeSpan time, VideoActionType afterVideoActionType, string serviceAddress)
        {
            var notificationService = new NetTcpWcfClientService<ITLVideo>(serviceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.ChangeFrame(time, afterVideoActionType));
            UnregWcfEvents(notificationService);
            return result;
        }
        #endregion
    }
}