// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Device.Contracts;
#endregion

namespace TLAuto.Device.ServerHost.IocService
{
    public interface ITLAutoDevicePushIocService
    {
        void RegistControlDeviceEx(string key, ControlInfo controlInfo, ITLAutoDevicePushCallback callBack);

        void UnRegistControlDeviceEx(string key, string serviceKey);
    }
}