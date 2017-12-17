// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Device.Contracts;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public class SendWcfCommandEx : NetTcpWcfClientService<ITLAutoDevice>
    {
        public SendWcfCommandEx(string serviceAddress)
            : base(serviceAddress) { }

        public async Task<WcfResultInfo> SendEx(string serviceKey, byte[] data)
        {
            var result = await SendAsync(async proxy => await proxy.ControlDevice(new ControlInfo
                                                                                  {
                                                                                      ServiceKey = serviceKey,
                                                                                      Data = data
                                                                                  }));
            return result;
        }
    }
}