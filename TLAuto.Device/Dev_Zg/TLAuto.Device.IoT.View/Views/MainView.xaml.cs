// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Device.Contracts;
using TLAuto.Device.IoT.ServiceData;
using TLAuto.Device.IoT.View.ViewModels;
#endregion

namespace TLAuto.Device.IoT.View.Views
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

        public async Task<WcfResultInfo> ControlDevice(IoTControlServiceData serviceData)
        {
            var task = await DispatcherHelper.UIDispatcher.InvokeAsync(async () =>
                                                                       {
                                                                           var mainViewModel = (MainViewModel)DataContext;
                                                                           var ioTSocketInfo = mainViewModel.IoTSocketInfos.FirstOrDefault(s => s.SignName == serviceData.SignName);
                                                                           if ((ioTSocketInfo != null) && !ioTSocketInfo.IsOpenScoket)
                                                                           {
                                                                               return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNotOpenSocketPort};
                                                                           }
                                                                           var plc = ioTSocketInfo?.IoTInfos.FirstOrDefault(s => s.DeviceNumber == serviceData.DeviceNumber);
                                                                           if (plc != null)
                                                                           {
                                                                               var result = await plc.ControlPLC(serviceData);
                                                                               return new WcfResultInfo {Data = result};
                                                                           }
                                                                           return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
                                                                       });
            return task.Result;
        }

        //public void RegistControlDeviceEx(string key, IoTControlServiceData serviceData, ITLAutoDevicePushCallback callBack)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
        //    {
        //        var mainViewModel = (MainViewModel)DataContext;
        //        var ioTSocketInfo = mainViewModel.IoTSocketInfos.FirstOrDefault(s => s.SignName == serviceData.SignName);
        //        if (ioTSocketInfo != null && ioTSocketInfo.IsOpenScoket)
        //        {
        //            ioTSocketInfo.Add(key, serviceData, callBack);
        //        }
        //    });
        //}

        //public void UnRegistControlDeviceEx(string key)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
        //    {
        //        var mainViewModel = (MainViewModel)DataContext;
        //        foreach (var ioTSocketInfo in mainViewModel.IoTSocketInfos)
        //        {
        //            ioTSocketInfo.Remove(key);
        //        }
        //    });
        //}
    }
}