// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;

using TLAuto.Device.Contracts;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public interface IDeviceService : IDisposable
    {
        string Description { get; }

        string ServiceKey { get; }

        IDeviceSettings DeviceSettings { get; }

        IDisposable View { get; }

        void Init(string wcfServiceAddress);

        Task<WcfResultInfo> ControlDevice(byte[] data);

        void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack);

        void UnRegistControlDeviceEx(string key);

        bool SaveDeviceSettings(out Exception exception);

        bool DeleteDeviceSettings(out Exception exception);
    }
}