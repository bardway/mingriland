// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Net.Sockets;
#endregion

namespace TLAuto.Base.Network.Sockets
{
    public static class SocketHelper
    {
        public static void SetKeepAlive(Socket socket)
        {
            //var inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };// 首次探测时间20 秒, 间隔侦测时间2 秒
            var inValue = new byte[] {1, 0, 0, 0, 0x88, 0x13, 0, 0, 0xd0, 0x07, 0, 0}; // 首次探测时间5 秒, 间隔侦测时间2 秒
            socket.IOControl(IOControlCode.KeepAliveValues, inValue, null);
        }
    }
}