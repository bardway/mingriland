// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;

using TLAuto.Machine.Plugins.HitMouse.Models.Attributes;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models.Enums
{
    public enum HitType
    {
        /// <summary>
        /// 默认（第一轮使用这个）
        /// </summary>
        [Description("HitDefault")]
        Default = 0,

        /// <summary>
        /// 斗转星移     进度充能灯加快
        /// </summary>
        [Description("HitBuff1")]
        [TextToMusic("已启动，斗转星移，下一轮游戏的进度灯充能速度将会加快")]
        Buff1 = 1,

        /// <summary>
        /// 八卦阵       同时点亮4盏梅花灯
        /// </summary>
        [Description("HitBuff2")]
        [TextToMusic("已启动，八卦阵，下一轮游戏将同时点亮4盏梅花桩")]
        Buff2 = 2,

        /// <summary>
        /// 心如止水     闪烁频率降低
        /// </summary>
        [Description("HitBuff3")]
        [TextToMusic("已启动，心如止水，下一轮游戏的梅花桩灯闪烁频率将会降低")]
        Buff3 = 3,

        /// <summary>
        /// 太极阵       点亮时间变长
        /// </summary>
        [Description("HitBuff4")]
        [TextToMusic("已启动，太极阵，下一轮游戏的梅花桩的点亮时间将会变长")]
        Buff4 = 4,

        /// <summary>
        /// 猪队友       点亮时间降低  频率加快
        /// </summary>
        [Description("HitBuff5")]
        [TextToMusic("发现，猪一样的队友，下一轮游戏的梅花桩的点亮时间降低，频率加快")]
        Buff5 = 5,

        /// <summary>
        /// 打酱油       相邻2个不能被点亮
        /// </summary>
        [Description("HitBuff6")]
        [TextToMusic("发现，打酱油者，下一轮游戏相邻的梅花桩将不再连续被点亮")]
        Buff6 = 6,

        /// <summary>
        /// 致盲         背景灯失灵
        /// </summary>
        [Description("HitBuff8")]
        [TextToMusic("撞墙致盲，下一轮游戏的灯光有时会失灵，请玩家注意安全")]
        Buff7 = 7,

        /// <summary>
        /// 致残         充能灯速度大幅降低
        /// </summary>
        [Description("HitBuff7")]
        [TextToMusic("摔伤致残，下一轮游戏的进度灯充能速度将会大幅降低")]
        Buff8 = 8
    }
}