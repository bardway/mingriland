// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.ServerHost.IocService
{
    public partial class WcfTLAutoDeviceIocService : ITLAutoDeviceIocService
    {
        public Task<bool> GetConnnectStatus()
        {
            return Task.Factory.StartNew(() => true);
        }

        public async Task<WcfResultInfo> ControlDevice(ControlInfo controlInfo)
        {
            return await TLDeviceExtensionsService.Instance.ControlDevice(controlInfo);
        }
    }
}