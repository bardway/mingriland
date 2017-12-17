// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ServiceModel;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Device.Contracts
{
    [ServiceContract]
    public interface ITLAutoDevice
    {
        [OperationContract]
        Task<bool> GetConnnectStatus();

        [OperationContract]
        Task<WcfResultInfo> ControlDevice(ControlInfo controlInfo);
    }
}