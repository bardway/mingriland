// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.DMX.ServiceData;
using TLAuto.Device.DMX.View.ViewModels;
#endregion

namespace TLAuto.Device.DMX.View.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, IDisposable
    {
        public MainView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            ((MainViewModel)DataContext).Cleanup();
        }

        public async Task<WcfResultInfo> ControlDevice(IEnumerable<DMXControlServiceData> controlServiceDatas)
        {
            var task = await DispatcherHelper.UIDispatcher.InvokeAsync(async () =>
                                                                       {
                                                                           var mainViewModel = (MainViewModel)DataContext;
                                                                           var channels = controlServiceDatas.Select(s => new Tuple<int, int>(s.ChannelNum, s.ChannelValue)).ToList();
                                                                           var tlDMXDevice = mainViewModel.TLDMXDevice;
                                                                           if (!tlDMXDevice.IsLoaded)
                                                                           {
                                                                               return new WcfResultInfo {ErrorMsg = "驱动未加载。"};
                                                                           }
                                                                           await mainViewModel.ControlMulitiSingleChannel(channels);
                                                                           return new WcfResultInfo {Data = new[] {true.ToByte()}};
                                                                       });
            return task.Result;
        }
    }
}