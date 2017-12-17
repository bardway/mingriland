// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
using TLAuto.Device.IoT.ServiceData;
using TLAuto.Log;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Device.IoT.View
{
    public static class SendWcfCommandHelper
    {
        public const string ErrorInfoForNoDevices = "没有找到任何可控制的IoT设备。";
        public const string ErrorInfoForNotOpenSocketPort = "没有打开Socket通信通道。";
        public const string ErrorInfoForCommandTimeOutOrException = "命令调用超时或出现异常";
        private static readonly LogWraper Log = new LogWraper("IoTSendWcfCommand");

        public static async Task<WcfResultInfo> Send(IoTControlServiceData serviceData)
        {
            var sendWcfCommand = new SendWcfCommandEx(IoTDeviceService.GetIoTDeviceService().WcfServiceAddress);
            sendWcfCommand.Error += SendWcfCommand_Error;
            sendWcfCommand.CommunicationError += SendWcfCommand_CommunicationError;
            sendWcfCommand.TimeoutError += SendWcfCommand_TimeoutError;
            var result = await sendWcfCommand.SendEx(IoTDeviceService.Key, serviceData.ToBytes());
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