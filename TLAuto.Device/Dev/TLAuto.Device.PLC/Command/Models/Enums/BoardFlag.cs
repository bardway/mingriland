// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.PLC.Command.Models.Enums
{
    [Flags]
    public enum BoardFlag
    {
        S0 = 0x01,
        S1 = 0x02,
        S2 = 0x04,
        S3 = 0x08,
        S4 = 0x10,
        S5 = 0x20,
        S6 = 0x40,
        S7 = 0x80
    }
}