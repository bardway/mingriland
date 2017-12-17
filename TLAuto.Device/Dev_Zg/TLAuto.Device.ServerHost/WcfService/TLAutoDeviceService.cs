// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Device.Contracts;
using TLAuto.Device.ServerHost.ViewModels;
#endregion

namespace TLAuto.Device.ServerHost.WcfService
{
    public class TLAutoDeviceService : ITLAutoDevice
    {
        public async Task<bool> GetConnnectStatus()
        {
            return await ViewModelLocator.Instance.TLAutoDeviceIocService.GetConnnectStatus();
        }

        public async Task<WcfResultInfo> ControlDevice(ControlInfo controlInfo)
        {
            return await ViewModelLocator.Instance.TLAutoDeviceIocService.ControlDevice(controlInfo);
        }
    }
}