// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Notification.Contracts;
using TLAuto.Wcf.Client;
using TLAuto.Wcf.Client.Events;

using SwitchItem = TLAuto.Machine.Controls.Models.SwitchItem;
#endregion

namespace TLAuto.Machine.App.Common
{
    public static class SendWcfCommandHelper
    {
        public static void WriteMsg(string msg)
        {
            Messenger.Default.Send(new NotificationMessage(msg));
        }

        public static async Task<bool> InvokerSimulationDigitalSwitch(SwitchItem boardItem)
        {
            var sendWcfCommand = new NetTcpWcfClientService<ITLAutoDevice>(boardItem.ServiceAddress);
            var result = await sendWcfCommand.SendAsync(async proxy =>
                                                        {
                                                            var controlInfo = new ControlInfo {ServiceKey = CommonConfigHelper.PLCServiceKey};
                                                            var serviceData = new PLCControlServiceData
                                                                              {
                                                                                  ControlPLCType = ControlPLCType.SimulationDigitalSwitch,
                                                                                  DeviceNumber = boardItem.DeviceNumber,
                                                                                  Number = new[] {boardItem.Number},
                                                                                  PortSignName = boardItem.SignName
                                                                              };
                                                            controlInfo.Data = serviceData.ToBytes();
                                                            var resultInfo = await proxy.ControlDevice(controlInfo);
                                                            if (!resultInfo.IsError && (resultInfo.Data != null))
                                                            {
                                                                var result1 = resultInfo.Data[0].ToBoolean();
                                                                return result1;
                                                            }
                                                            return false;
                                                        });
            return result;
        }

        public static async Task<bool> InvokerNotificationForStart(string notificationKey)
        {
            var notificationService = new NetTcpWcfClientService<ITLNotification>(ConfigHelper.NotificationServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.AddAppStatus(notificationKey, AppStatusType.Start));
            UnregWcfEvents(notificationService);
            return result;
        }

        public static async Task<bool> InvokerNotificationForStop(string notificationKey)
        {
            var notificationService = new NetTcpWcfClientService<ITLNotification>(ConfigHelper.NotificationServiceAddress);
            RegWcfEvents(notificationService);
            var result = await notificationService.SendAsync(async proxy => await proxy.AddAppStatus(notificationKey, AppStatusType.Stop));
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
            WriteMsg(e.Msg);
        }

        private static void WcfClientService_Error(object sender, WcfClientServiceErrorEventArgs e)
        {
            WriteMsg(e.Msg);
        }

        private static void WcfClientService_CommunicationError(object sender, WcfClientServiceErrorEventArgs e)
        {
            WriteMsg(e.Msg);
        }
    }
}