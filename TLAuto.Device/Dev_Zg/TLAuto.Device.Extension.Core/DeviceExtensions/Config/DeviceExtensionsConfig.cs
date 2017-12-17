// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Device.Extension.Core.DeviceExtensions.Config
{
    public class DeviceExtensionsConfig
    {
        public List<DeviceDllConfig> DeviceConfigs { get; } = new List<DeviceDllConfig>();
    }
}