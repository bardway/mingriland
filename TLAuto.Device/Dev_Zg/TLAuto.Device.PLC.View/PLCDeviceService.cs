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
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Device.PLC.View.Config;
using TLAuto.Device.PLC.View.Models;
using TLAuto.Device.PLC.View.ViewModels;
using TLAuto.Device.PLC.View.Views;
#endregion

namespace TLAuto.Device.PLC.View
{
    [Export(typeof(IDeviceService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class PLCDeviceService : DeviceService<PLCDeviceSettings>
    {
        internal const string Key = "PLC_4bf1a873-3cd9-41a8-a883-bf3eba7d396d";

        protected override IDisposable GetView()
        {
            return new MainView();
        }

        public override async Task<WcfResultInfo> ControlDevice(byte[] data)
        {
            var serviceData = data.ToObject<PLCControlServiceData>();
            if (View != null)
            {
                return await((MainView)View).ControlDevice(serviceData);
            }
            return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
        }

        //public override void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack)
        //{
        //    var serviceData = data.ToObject<PLCControlServiceData>();
        //    ((MainView)View)?.RegistControlDeviceEx(key, serviceData, callBack);
        //}

        //public override void UnRegistControlDeviceEx(string key)
        //{
        //    ((MainView)View)?.UnRegistControlDeviceEx(key);
        //}

        internal static PLCDeviceService GetPLCDeviceService()
        {
            return (PLCDeviceService)TLDeviceExtensionsService.Instance.GetDeviceService(Key);
        }

        internal static MainViewModel GetMainViewModel()
        {
            var deviceService = GetPLCDeviceService();
            return (MainViewModel)((MainView)deviceService.View)?.DataContext;
        }

        internal static PLCSerialPortInfo GetPLCSerialPortInfo(string portName)
        {
            var viewModel = GetMainViewModel();
            return viewModel.PLCSerialPortInfos.FirstOrDefault(plcSerialPortInfo => plcSerialPortInfo.PortName == portName);
        }

        internal static PLCDeviceInfo GetPLCDeviceInfo(string portName, int deviceNumber)
        {
            var viewModel = GetMainViewModel();
            return viewModel.PLCSerialPortInfos.Where(plcSerialPortInfo => plcSerialPortInfo.PortName == portName).SelectMany(plcSerialPortInfo => plcSerialPortInfo.PLCInfos).FirstOrDefault(plcDeviceInfo => plcDeviceInfo.DeviceNumber == deviceNumber);
        }

        internal static bool SaveSettings()
        {
            var deviceService = GetPLCDeviceService();
            Exception ex;
            if (deviceService.SaveDeviceSettings(out ex))
            {
                return true;
            }
            MessageBox.Show(ex.Message);
            return false;
        }

        #region Properties
        public override string Description { get; } = "工控板";

        public override string ServiceKey { get; } = Key;
        #endregion
    }
}