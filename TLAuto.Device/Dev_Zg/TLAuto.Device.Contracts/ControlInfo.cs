// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.Contracts
{
    public class ControlInfo
    {
        /// <summary>
        /// 设备通信服务唯一标识符
        /// </summary>
        public string ServiceKey { set; get; }

        /// <summary>
        /// 数据包
        /// </summary>
        public byte[] Data { set; get; }
    }
}