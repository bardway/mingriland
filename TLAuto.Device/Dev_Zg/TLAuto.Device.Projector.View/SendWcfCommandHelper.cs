// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
using TLAuto.Device.Projector.ServiceData;
using TLAuto.Log;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Device.Projector.View
{
    public static class SendWcfCommandHelper
    {
        public const string ErrorInfoForNoDevices = "没有找到任何可控制的投影仪设备。";
        public const string ErrorInfoForNotOpenSerialPort = "没有打开串口通信通道。";
        public const string ErrorInfoForCommandTimeOutOrException = "命令调用超时或出现异常";
        private static readonly LogWraper Log = new LogWraper("ProjectorSendWcfCommand");

        public static async Task<WcfResultInfo> Send(ProjectorControlServiceData serviceData)
        {
            var sendWcfCommand = new SendWcfCommandEx(ProjectorDeviceService.GetProjectorDeviceService().WcfServiceAddress);
            sendWcfCommand.Error += SendWcfCommand_Error;
            sendWcfCommand.CommunicationError += SendWcfCommand_CommunicationError;
            sendWcfCommand.TimeoutError += SendWcfCommand_TimeoutError;
            var result = await sendWcfCommand.SendEx(ProjectorDeviceService.Key, serviceData.ToBytes());
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