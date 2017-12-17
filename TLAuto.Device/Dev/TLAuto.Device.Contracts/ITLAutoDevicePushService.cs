// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel;
#endregion

namespace TLAuto.Device.Contracts
{
    [ServiceContract(CallbackContract = typeof(ITLAutoDevicePushCallback))]
    public interface ITLAutoDevicePushService
    {
        [OperationContract(IsOneWay = true)]
        void RegistControlDeviceEx(string key, ControlInfo controlInfo);

        [OperationContract(IsOneWay = true)]
        void UnRegistControlDeviceEx(string key, string serviceKey);
    }
}