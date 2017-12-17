// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
using TLAuto.Device.IoT.ServiceData;
using TLAuto.Device.IoT.View.Config;
using TLAuto.Device.IoT.View.Models;
using TLAuto.Device.IoT.View.ViewModels;
using TLAuto.Device.IoT.View.Views;
#endregion

namespace TLAuto.Device.IoT.View
{
    [Export(typeof(IDeviceService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class IoTDeviceService : DeviceService<IoTDeviceSettings>
    {
        internal const string Key = "IoT_018d2b09-79f2-4158-81d9-9654f59518b7";

        public override string Description { get; } = "IoT";

        public override string ServiceKey { get; } = Key;

        protected override IDisposable GetView()
        {
            return new MainView();
        }

        public override async Task<WcfResultInfo> ControlDevice(byte[] data)
        {
            var serviceData = data.ToObject<IoTControlServiceData>();
            if (View != null)
            {
                return await ((MainView)View).ControlDevice(serviceData);
            }
            return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
        }

        //public override void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack)
        //{
        //    var serviceData = data.ToObject<IoTControlServiceData>();
        //    ((MainView)View)?.RegistControlDeviceEx(key, serviceData, callBack);
        //}

        //public override void UnRegistControlDeviceEx(string key)
        //{
        //    ((MainView)View)?.UnRegistControlDeviceEx(key);
        //}

        internal static IoTDeviceService GetIoTDeviceService()
        {
            return (IoTDeviceService)TLDeviceExtensionsService.Instance.GetDeviceService(Key);
        }

        internal static IoTSocketInfo GetIoTSocketInfo(string signName)
        {
            var viewModel = GetMainViewModel();
            return viewModel.IoTSocketInfos.FirstOrDefault(ioTSocketInfo => ioTSocketInfo.SignName == signName);
        }

        internal static MainViewModel GetMainViewModel()
        {
            var deviceService = GetIoTDeviceService();
            return (MainViewModel)((MainView)deviceService.View)?.DataContext;
        }

        internal static bool SaveSettings()
        {
            var deviceService = GetIoTDeviceService();
            Exception ex;
            if (deviceService.SaveDeviceSettings(out ex))
            {
                return true;
            }
            MessageBox.Show(ex.Message);
            return false;
        }
    }
}