// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.DMX.ServiceData;
using TLAuto.Device.Extension.Core;
using TLAuto.Log;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Device.DMX.View
{
    public static class SendWcfCommandHelper
    {
        public const string ErrorInfoForNoDevices = "没有找到任何可控制的工控板设备。";
        public const string ErrorInfoForQueryFailed = "查询命令调用失败";
        private static readonly LogWraper Log = new LogWraper("DMXSendWcfCommand");

        public static async Task<WcfResultInfo> Send(IEnumerable<DMXControlServiceData> serviceDatas)
        {
            var sendWcfCommand = new SendWcfCommandEx(DMXDeviceService.GetDMXDeviceService().WcfServiceAddress);
            sendWcfCommand.Error += SendWcfCommand_Error;
            sendWcfCommand.CommunicationError += SendWcfCommand_CommunicationError;
            sendWcfCommand.TimeoutError += SendWcfCommand_TimeoutError;
            var result = await sendWcfCommand.SendEx(DMXDeviceService.Key, serviceDatas.ToBytes());
            sendWcfCommand.Error -= SendWcfCommand_Error;
            sendWcfCommand.CommunicationError -= SendWcfCommand_CommunicationError;
            sendWcfCommand.TimeoutError -= SendWcfCommand_TimeoutError;
            return result;
        }

        private static void SendWcfCommand_TimeoutError(object sender, WcfClientServiceErrorEventArgs e)
        {
            Log.Critical(e.Msg, e.Ex);
        }

        private static void SendWcfCommand_CommunicationError(object sender, WcfClientServiceErrorEventArgs e)
        {
            Log.Critical(e.Msg, e.Ex);
        }

        private static void SendWcfCommand_Error(object sender, WcfClientServiceErrorEventArgs e)
        {
            Log.Critical(e.Msg, e.Ex);
        }
    }
}