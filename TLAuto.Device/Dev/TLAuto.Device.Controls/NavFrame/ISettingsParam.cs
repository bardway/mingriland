// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Controls.NavFrame
{
    public interface ISettingsParam
    {
        Uri PageUri { get; }

        string HeaderName { get; }
    }
}