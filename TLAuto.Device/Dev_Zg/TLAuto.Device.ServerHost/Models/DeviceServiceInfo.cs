// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight;

using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.ServerHost.Models
{
    public class DeviceServiceInfo : ViewModelBase
    {
        public DeviceServiceInfo(IDeviceService deviceService)
        {
            DeviceService = deviceService;
            Description = DeviceService.Description;
        }

        #region Methods
        public override void Cleanup()
        {
            DeviceService.Dispose();
            DeviceService = null;
            base.Cleanup();
        }
        #endregion

        #region Properties
        public string Description { get; }

        public IDeviceService DeviceService { private set; get; }
        #endregion
    }
}