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
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.HitCheckTask
{
    public sealed class HitCheckTask
    {
        private readonly int _devicenumber;
        private readonly string _regKey;

        private readonly Timer _regTimer = new Timer(9 * 60 * 1000)
                                           {
                                               AutoReset = false
                                           };

        private readonly int _sendTimeout;
        private readonly string _serviceAddress;
        private readonly string _signName;

        private TLAutoDevicePushCallback _callback;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfClientService;

        public HitCheckTask(string regKey, MachineButtonItem button, int sendTimeout = 60000 * 10)
        {
            _regKey = regKey;
            MachineButtonItem = button;
            _serviceAddress = button.ServiceAddress;
            _devicenumber = button.DeviceNumber;
            _signName = button.SignName;
            _sendTimeout = sendTimeout;
            _regTimer.Elapsed += RegTimer_Elapsed;
        }

        public MachineButtonItem MachineButtonItem { get; }

        public bool IsReg { private set; get; }

        private void RegTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Unreg();
            Reg();
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
                                                                                                   DeviceNumber = _devicenumber,
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

        private bool Unreg()
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
            var stamp = Guid.NewGuid();
            //Debug.WriteLine($"STAMP: {stamp.ToString().Remove(8)} SWITCHNUMBER: {switchItem.SwitchNumber} DEVICENUMBER: {_devicenumber} SWITCHSTATUS: {switchItem.SwitchStatus}");
            var switchItemWithDeviceNumber = new SwitchItemWithDeviceNumber
                                             {
                                                 Stamp = stamp,
                                                 SwitchItem = switchItem,
                                                 DeviceNumber = _devicenumber
                                             };
            HitTaskCache.PushMessage(switchItemWithDeviceNumber);
            OnNotifySwitchItem();
        }

        public event EventHandler NotifySwitchItem;

        private void OnNotifySwitchItem()
        {
            NotifySwitchItem?.Invoke(this, EventArgs.Empty);
        }
    }
}