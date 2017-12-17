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
using TLAuto.Device.Projector.ServiceData;
using TLAuto.Device.Projector.View.Config;
using TLAuto.Device.Projector.View.Models;
using TLAuto.Device.Projector.View.ViewModels;
using TLAuto.Device.Projector.View.Views;
#endregion

namespace TLAuto.Device.Projector.View
{
    [Export(typeof(IDeviceService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ProjectorDeviceService : DeviceService<ProjectorDeviceSettings>
    {
        internal const string Key = "Projector_c3000df2_2c26_4107_af0e_c877330e97cd";

        protected override IDisposable GetView()
        {
            return new MainView();
        }

        public override async Task<WcfResultInfo> ControlDevice(byte[] data)
        {
            var serviceData = data.ToObject<ProjectorControlServiceData>();
            if (View != null)
            {
                return await ((MainView)View).ControlDevice(serviceData);
            }
            return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
        }

        public override void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack) { }

        public override void UnRegistControlDeviceEx(string key) { }

        internal static ProjectorDeviceService GetProjectorDeviceService()
        {
            return (ProjectorDeviceService)TLDeviceExtensionsService.Instance.GetDeviceService(Key);
        }

        internal static MainViewModel GetMainViewModel()
        {
            var deviceService = GetProjectorDeviceService();
            return (MainViewModel)((MainView)deviceService.View)?.DataContext;
        }

        internal static ProjectorSerialPortInfo GetProjectorSerialPortInfo(string portName)
        {
            var viewModel = GetMainViewModel();
            return viewModel.ProjectorSerialPortInfos.FirstOrDefault(projectorSerialPortInfo => projectorSerialPortInfo.PortName == portName);
        }

        internal static bool SaveSettings()
        {
            var deviceService = GetProjectorDeviceService();
            Exception ex;
            if (deviceService.SaveDeviceSettings(out ex))
            {
                return true;
            }
            MessageBox.Show(ex.Message);
            return false;
        }

        #region Properties
        public override string Description { get; } = "投影仪";

        public override string ServiceKey { get; } = Key;
        #endregion
    }
}