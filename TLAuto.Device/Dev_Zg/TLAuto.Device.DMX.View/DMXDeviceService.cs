// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.DMX.ServiceData;
using TLAuto.Device.DMX.View.Config;
using TLAuto.Device.DMX.View.Models;
using TLAuto.Device.DMX.View.ViewModels;
using TLAuto.Device.DMX.View.Views;
using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.DMX.View
{
    [Export(typeof(IDeviceService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DMXDeviceService : DeviceService<DMXDeviceSettings>
    {
        internal const string Key = "DMX_92c28f89-5dfd-4166-ab51-1e08599ffdfb";

        public override string Description { get; } = "舞台灯光控制";

        public override string ServiceKey { get; } = Key;

        protected override IDisposable GetView()
        {
            return new MainView();
        }

        public override async Task<WcfResultInfo> ControlDevice(byte[] data)
        {
            var controlServiceDatas = data.ToObject<List<DMXControlServiceData>>();
            if (View != null)
            {
                return await((MainView)View).ControlDevice(controlServiceDatas);
            }
            return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
        }

        //public override void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack)
        //{

        //}

        //public override void UnRegistControlDeviceEx(string key)
        //{

        //}

        internal static DMXDeviceService GetDMXDeviceService()
        {
            return (DMXDeviceService)TLDeviceExtensionsService.Instance.GetDeviceService(Key);
        }

        internal static MainViewModel GetMainViewModel()
        {
            var deviceService = GetDMXDeviceService();
            return (MainViewModel)((MainView)deviceService.View)?.DataContext;
        }

        internal static DMXDeviceInfo GetDMXDeviceInfo(int channelBegin)
        {
            var viewModel = GetMainViewModel();
            return viewModel.DeviceInfos.FirstOrDefault(dmxDeviceInfo => dmxDeviceInfo.ChannelBegin == channelBegin);
        }

        internal static bool SaveSettings()
        {
            var deviceService = GetDMXDeviceService();
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