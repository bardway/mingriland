// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Device.PLC.View.ViewModels;
#endregion

namespace TLAuto.Device.PLC.View.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : IDisposable
    {
        public MainView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            ((MainViewModel)DataContext).Cleanup();
        }

        public async Task<WcfResultInfo> ControlDevice(PLCControlServiceData serviceData)
        {
            var task = await DispatcherHelper.UIDispatcher.InvokeAsync(async () =>
                                                                       {
                                                                           var mainViewModel = (MainViewModel)DataContext;
                                                                           var serialPortInfo = mainViewModel.PLCSerialPortInfos.FirstOrDefault(s => s.PortSignName == serviceData.PortSignName);
                                                                           if ((serialPortInfo != null) && !serialPortInfo.IsOpenSerialPort)
                                                                           {
                                                                               return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNotOpenSerialPort};
                                                                           }
                                                                           var plc = serialPortInfo?.PLCInfos.FirstOrDefault(s => s.DeviceNumber == serviceData.DeviceNumber);
                                                                           if (plc != null)
                                                                           {
                                                                               var result = await plc.ControlPLC(serviceData);
                                                                               return new WcfResultInfo {Data = result};
                                                                           }
                                                                           return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
                                                                       });
            return task.Result;
        }

        //public void RegistControlDeviceEx(string key, PLCControlServiceData serviceData, ITLAutoDevicePushCallback callBack)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
        //    {
        //        var mainViewModel = (MainViewModel)DataContext;
        //        var serialPortInfo = mainViewModel.PLCSerialPortInfos.FirstOrDefault(s => s.PortSignName == serviceData.PortSignName);
        //        if (serialPortInfo != null && serialPortInfo.IsOpenSerialPort)
        //        {
        //            serialPortInfo.Add(key, serviceData, callBack);
        //        }
        //    });
        //}

        //public void UnRegistControlDeviceEx(string key)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
        //    {
        //        var mainViewModel = (MainViewModel)DataContext;
        //        foreach (var plcSerialPortInfo in mainViewModel.PLCSerialPortInfos)
        //        {
        //            plcSerialPortInfo.Remove(key);
        //        }
        //    });
        //}
    }
}