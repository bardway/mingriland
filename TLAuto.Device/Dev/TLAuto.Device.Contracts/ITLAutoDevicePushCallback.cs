// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel;
#endregion

namespace TLAuto.Device.Contracts
{
    [ServiceContract]
    public interface ITLAutoDevicePushCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(WcfResultInfo wcfResutlInfo);
    }
}