// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Log;
using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Device.PLC.View
{
    public static class SendWcfCommandHelper
    {
        public const string ErrorInfoForNoDevices = "没有找到任何可控制的工控板设备。";
        public const string ErrorInfoForNotOpenSerialPort = "没有打开串口通信通道。";
        public const string ErrorInfoForQueryFailed = "查询命令调用失败";
        private static readonly LogWraper Log = new LogWraper("PLCSendWcfCommand");

        public static async Task<WcfResultInfo> Send(PLCControlServiceData serviceData)
        {
            var sendWcfCommand = new SendWcfCommandEx(PLCDeviceService.GetPLCDeviceService().WcfServiceAddress);
            sendWcfCommand.Error += SendWcfCommand_Error;
            sendWcfCommand.CommunicationError += SendWcfCommand_CommunicationError;
            sendWcfCommand.TimeoutError += SendWcfCommand_TimeoutError;
            var result = await sendWcfCommand.SendEx(PLCDeviceService.Key, serviceData.ToBytes());
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