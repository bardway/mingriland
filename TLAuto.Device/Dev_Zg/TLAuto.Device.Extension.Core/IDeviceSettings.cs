// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public interface IDeviceSettings
    {
        bool Exists { get; }

        bool Delete(out Exception exception);

        bool Save(Type type, out Exception exception);
    }
}