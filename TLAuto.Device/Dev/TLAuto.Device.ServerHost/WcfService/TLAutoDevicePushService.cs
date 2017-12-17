// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel;

using TLAuto.Device.Contracts;
using TLAuto.Device.ServerHost.ViewModels;
#endregion

namespace TLAuto.Device.ServerHost.WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class TLAutoDeviceService : ITLAutoDevicePushService
    {
        public void RegistControlDeviceEx(string key, ControlInfo controlInfo)
        {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<ITLAutoDevicePushCallback>();
            ViewModelLocator.Instance.TLAutoDevicePushIocService.RegistControlDeviceEx(key, controlInfo, callbackChannel);
        }

        public void UnRegistControlDeviceEx(string key, string serviceKey)
        {
            ViewModelLocator.Instance.TLAutoDevicePushIocService.UnRegistControlDeviceEx(key, serviceKey);
        }
    }
}