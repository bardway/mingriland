// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;

using TLAuto.Base.Async;
using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Log;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.Core.AsyncTask
{
    public class NotificationButtonPressAsyncTask : AsyncTaskBase<MachineButtonItem>
    {
        private const string RegKey = nameof(NotificationButtonPressAsyncTask);
        private readonly object _lockObj = new object();
        private readonly LogWraper _log = new LogWraper(nameof(NotificationButtonPressAsyncTask));
        private readonly TimeSpan _sendTimeout;
        private TLAutoDevicePushCallback _callback = new TLAutoDevicePushCallback();
        private List<MachineButtonItem> _checkButtonItems;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        public NotificationButtonPressAsyncTask(List<MachineButtonItem> checkButtonItems, int timeOutMs)
            : base(null, timeOutMs)
        {
            _callback.Notify += Callback_Notify;
            _checkButtonItems = checkButtonItems;
            _sendTimeout = TimeSpan.FromMilliseconds(timeOutMs);
        }

        private void Callback_Notify(object sender, WcfResultInfo e)
        {
            lock (_lockObj)
            {
                if (_checkButtonItems != null)
                {
                    if (e.IsError)
                    {
                        PostResult(null);
                    }
                    var switchItem = e.Data.ToObject<SwitchItem>();
                    var findItem = _checkButtonItems.Find(s => s.Number == switchItem.SwitchNumber);
                    if (findItem != null)
                    {
                        PostResult(findItem);
                    }
                }
            }
        }

        protected override void Invoke()
        {
            var checkButtonItem = _checkButtonItems[0];
            try
            {
                _wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, checkButtonItem.ServiceAddress, _sendTimeout);
                var result = _wcfService.Send(proxy =>
                                              {
                                                  proxy.RegistControlDeviceEx(RegKey,
                                                                              new ControlInfo
                                                                              {
                                                                                  ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                  Data = new PLCControlServiceData
                                                                                         {
                                                                                             ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                             DeviceNumber = checkButtonItem.DeviceNumber,
                                                                                             PortSignName = checkButtonItem.SignName
                                                                                         }.ToBytes()
                                                                              });
                                              });
                if (!result)
                {
                    PostResult(null);
                }
            }
            catch (Exception ex)
            {
                PostResult(null);
            }
        }

        protected override void Exception(Exception ex)
        {
            _log.Error(ex.Message, ex);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                lock (_lockObj)
                {
                    var checkButtonItem = _checkButtonItems[0];
                    _wcfService.Close();
                    var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback,
                                                                                             checkButtonItem.ServiceAddress);
                    service.Send(proxy => { proxy.UnRegistControlDeviceEx(RegKey, CommonConfigHelper.PLCServiceKey); });
                    service.Close();
                    _checkButtonItems = null;
                    _callback.Notify -= Callback_Notify;
                    _callback = null;
                }
            }
            catch (Exception ex)
            {
                _log.Critical(ex.Message);
            }
        }
    }
}