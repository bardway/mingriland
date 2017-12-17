// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Timers;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.Boxing
{
    public class HitCheckTask
    {
        private readonly int _deviceNumber;
        private readonly string _regKey;
        private readonly Timer _regTimer = new Timer(1000) {AutoReset = false};
        private readonly int _sendTimeout;
        private readonly string _serviceAddress;
        private readonly string _signName;
        private TLAutoDevicePushCallback _callback;
        private int _timeCount;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfClientService;

        public HitCheckTask(string regKey, string serviceAddress, int deviceNumber, string signName, int sendTimeout = 60000 * 10)
        {
            _regKey = regKey;
            _serviceAddress = serviceAddress;
            _deviceNumber = deviceNumber;
            _signName = signName;
            _sendTimeout = sendTimeout;
            _regTimer.Elapsed += RegTimer_Elapsed;
        }

        public bool IsReg { private set; get; }

        private void RegTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timeCount++;
            if (_timeCount == 60000 * 9)
            {
                Unreg();
                Reg();
            }
            _regTimer.Start();
        }

        public bool Reg()
        {
            try
            {
                _callback = new TLAutoDevicePushCallback();
                _callback.Notify += Callback_Notify;
                _wcfClientService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, _serviceAddress, TimeSpan.FromMilliseconds(_sendTimeout));
                var result = _wcfClientService.Send(proxy =>
                                                    {
                                                        proxy.RegistControlDeviceEx(_regKey,
                                                                                    new ControlInfo
                                                                                    {
                                                                                        ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                        Data = new PLCControlServiceData
                                                                                               {
                                                                                                   ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                                   DeviceNumber = _deviceNumber,
                                                                                                   PortSignName = _signName
                                                                                               }.ToBytes()
                                                                                    });
                                                    });
                IsReg = result;
                return result;
            }
            catch (Exception ex)
            {
                IsReg = false;
                return false;
            }
        }

        public bool Unreg()
        {
            try
            {
                if (_callback != null)
                {
                    _callback.Notify -= Callback_Notify;
                }
                _wcfClientService.Close();
                var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, _serviceAddress);
                service.Send(proxy => { proxy.UnRegistControlDeviceEx(_regKey, CommonConfigHelper.PLCServiceKey); });
                service.Close();
                IsReg = true;
                return true;
            }
            catch (Exception ex)
            {
                IsReg = false;
                return false;
            }
        }

        private void Callback_Notify(object sender, WcfResultInfo e)
        {
            if (e.IsError)
            {
                return;
            }
            var switchItem = e.Data.ToObject<SwitchItem>();
            OnNotifySwitchItem(switchItem);
        }

        public event EventHandler<SwitchItem> NotifySwitchItem;

        protected virtual void OnNotifySwitchItem(SwitchItem e)
        {
            NotifySwitchItem?.Invoke(this, e);
        }
    }
}