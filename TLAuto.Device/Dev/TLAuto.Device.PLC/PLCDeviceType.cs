// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
#endregion

namespace TLAuto.Device.PLC
{
    public enum PLCDeviceType
    {
        /// <summary>
        /// 自定义协议1，淘宝大连赛伟电子控制协议32输入16输出
        /// </summary>
        [Description("大连赛伟电子控制协议32输入16输出")]
        Custom1,

        /// <summary>
        /// 自定义协议2，淘宝大连赛伟电子控制协议12输入12输出
        /// </summary>
        [Description("大连赛伟电子控制协议12输入12输出")]
        Custom2,

        /// <summary>
        /// 自定义协议3，淘宝大连赛伟电子控制协议4输入4输出
        /// </summary>
        [Description("大连赛伟电子控制协议4输入4输出")]
        Custom3,

        /// <summary>
        /// 自定义协议4，卓越物联贰号店控制协议16输出
        /// </summary>
        [Description("卓越物联贰号店控制协议16输出")]
        Custom4,

        /// <summary>
        /// 通用PLC控制协议
        /// </summary>
        [Description("ModbusRTU")]
        ModbusRTU
    }
}