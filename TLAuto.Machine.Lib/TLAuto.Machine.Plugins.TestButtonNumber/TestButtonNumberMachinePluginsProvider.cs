// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.TestButtonNumber
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TestButtonNumberMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "TestButtonNumber";
        private readonly string _plcServiceAddress = "net.tcp://192.168.3.9/xingmen";
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            RegButtonCheckNotification(1, "国王开关检测1");
            RegButtonCheckNotification(2, "国王开关检测2");
            RegButtonCheckNotification(3, "国王开关检测3");
            RegButtonCheckNotification(4, "国王开关检测4");
        }

        private bool RegButtonCheckNotification(int deviceNumber, string signName)
        {
            try
            {
                var callback = new TLAutoDevicePushCallback();
                callback.Notify += (sen, eve) =>
                                   {
                                       if (!eve.IsError)
                                       {
                                           var switchItem = eve.Data.ToObject<SwitchItem>();
                                           var main = (Main)View;
                                           main.Dispatcher.BeginInvoke(new Action(() =>
                                                                                  {
                                                                                      main.TxtInfo.Text += $"{DateTime.Now} 开关号：{switchItem.SwitchNumber} 设备号：{deviceNumber} \r\n";
                                                                                      main.TxtInfo.ScrollToEnd();
                                                                                  }));
                                       }
                                   };
                _wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, _plcServiceAddress, TimeSpan.FromMinutes(99));
                var result = _wcfService.Send(proxy =>
                                              {
                                                  proxy.RegistControlDeviceEx(SignKey,
                                                                              new ControlInfo
                                                                              {
                                                                                  ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                  Data = new PLCControlServiceData
                                                                                         {
                                                                                             ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                             DeviceNumber = deviceNumber,
                                                                                             PortSignName = signName
                                                                                         }.ToBytes()
                                                                              });
                                              });
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //private void UnregButtonCheckNotification()
        //{
        //    try
        //    {
        //        _wcfService.Close();
        //        var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, _plcServiceAddress);
        //        service.Send(proxy =>
        //        {
        //            proxy.UnRegistControlDeviceEx(SignKey, CommonConfigHelper.PLCServiceKey);
        //        });
        //        service.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //private void Callback_Notify(object sender, WcfResultInfo e)
        //{
        //    if (!e.IsError)
        //    {
        //        var switchItem = e.Data.ToObject<SwitchItem>();

        //    }
        //}
    }
}