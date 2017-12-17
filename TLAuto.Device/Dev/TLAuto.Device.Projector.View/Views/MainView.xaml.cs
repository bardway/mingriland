// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Projector.ServiceData;
using TLAuto.Device.Projector.View.ViewModels;
#endregion

namespace TLAuto.Device.Projector.View.Views
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

        public async Task<WcfResultInfo> ControlDevice(ProjectorControlServiceData serviceData)
        {
            var task = await DispatcherHelper.UIDispatcher.InvokeAsync(async () =>
                                                                       {
                                                                           var mainViewModel = (MainViewModel)DataContext;
                                                                           var serialPortInfo = mainViewModel.ProjectorSerialPortInfos.FirstOrDefault(s => s.PortSignName == serviceData.PortSignName);
                                                                           if ((serialPortInfo != null) && !serialPortInfo.IsOpenSerialPort)
                                                                           {
                                                                               return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNotOpenSerialPort};
                                                                           }
                                                                           var projector = serialPortInfo?.ProjectorInfos.FirstOrDefault(s => s.DeviceNumber == serviceData.DeviceNumber);
                                                                           if (projector != null)
                                                                           {
                                                                               bool result;
                                                                               if (serviceData.PowerOnOrOff)
                                                                               {
                                                                                   result = await projector.PowerOn();
                                                                               }
                                                                               else
                                                                               {
                                                                                   result = await projector.PowerOff();
                                                                               }
                                                                               return new WcfResultInfo {Data = new[] {result.ToByte()}};
                                                                           }
                                                                           return new WcfResultInfo {ErrorMsg = SendWcfCommandHelper.ErrorInfoForNoDevices};
                                                                       });
            return task.Result;
        }
    }
}