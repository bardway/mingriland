// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;

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
    public class CheckButtonPressAndControlAsyncTask : AsyncTaskBase<bool>
    {
        private const string RegKey = nameof(CheckButtonPressAndControlAsyncTask);
        private readonly object _lockObj = new object();
        private readonly LogWraper _log = new LogWraper(nameof(CheckButtonPressAndControlAsyncTask));
        private readonly Dictionary<int, bool> _raiseButtonItems = new Dictionary<int, bool>();
        private List<MachineButtonItem> _allCheckButtonItems;
        private TLAutoDevicePushCallback _callback = new TLAutoDevicePushCallback();
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfClientService;

        public CheckButtonPressAndControlAsyncTask
        (
            List<MachineButtonItem> allCheckButtonItems,
            List<MachineButtonItem> raiseButtonItems,
            int timeOutMs) :
            base(null, timeOutMs)
        {
            _callback.Notify += Callback_Notify;
            _allCheckButtonItems = allCheckButtonItems;
            foreach (var machineButtonItem in raiseButtonItems)
            {
                _raiseButtonItems.Add(machineButtonItem.Number, false);
            }
        }

        private void Callback_Notify(object sender, WcfResultInfo e)
        {
            lock (_lockObj)
            {
                if (e.IsError)
                {
                    PostResult(false);
                }
                var switchItem = e.Data.ToObject<SwitchItem>();
                if (_raiseButtonItems.ContainsKey(switchItem.SwitchNumber))
                {
                    _raiseButtonItems[switchItem.SwitchNumber] = true;
                    var result = _raiseButtonItems.Values.All(s => s);
                    if (result)
                    {
                        PostResult(true);
                    }
                }
                else
                {
                    if (_allCheckButtonItems != null)
                    {
                        var result = _allCheckButtonItems.Any(s => s.Number == switchItem.SwitchNumber);
                        if (result)
                        {
                            PostResult(false);
                        }
                    }
                }
            }
        }

        protected override void Invoke()
        {
            var item = _allCheckButtonItems[0];
            try
            {
                _wcfClientService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback, item.ServiceAddress);
                var result = _wcfClientService.Send(proxy =>
                                                    {
                                                        proxy.RegistControlDeviceEx(RegKey,
                                                                                    new ControlInfo
                                                                                    {
                                                                                        ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                        Data = new PLCControlServiceData
                                                                                               {
                                                                                                   ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                                   DeviceNumber = item.DeviceNumber,
                                                                                                   PortSignName = item.SignName
                                                                                               }.ToBytes()
                                                                                    });
                                                    });
                if (!result)
                {
                    PostResult(false);
                }
            }
            catch (Exception ex)
            {
                PostResult(false);
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
                    try
                    {
                        _wcfClientService.Close();
                        var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(_callback,
                                                                                                 _allCheckButtonItems[0].ServiceAddress);
                        service.Send(proxy => { proxy.UnRegistControlDeviceEx(RegKey, CommonConfigHelper.PLCServiceKey); });
                        service.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.Critical(ex.Message);
                    }
                    _allCheckButtonItems = null;
                    _raiseButtonItems.Clear();
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