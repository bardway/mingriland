// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Base.Async;
using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Wcf.Client;
#endregion

namespace TLAuto.Machine.Plugins.HitMouse.Models.Behaviors
{
    public static class HitTask
    {
        private static HitData _currentHitData;
        private static readonly AsyncLock LockAsync = new AsyncLock();
        private static NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService;

        public static async Task HitActionForDefault(List<HitData> hitDatas, HitMouseMachinePluginsProvider provider, int chargeSpeed = 1, bool isAdjoin = false)
        {
            var callback = new TLAutoDevicePushCallback();
            RegButtonCheckNotification(provider._pointLightButtonItems, callback);
            callback.Notify += async (sen, eve) =>
                               {
                                   if (eve.IsError)
                                   {
                                       return;
                                   }
                                   var switchItem = eve.Data.ToObject<SwitchItem>();
                                   using (var releaser = await LockAsync.LockAsync())
                                   {
                                       if (_currentHitData?.Func != null)
                                       {
                                           await _currentHitData.Func(switchItem.SwitchNumber);
                                       }
                                   }
                               };
            var hitDataIndex = 0;
            foreach (var hitData in hitDatas)
            {
                var chargeSpeedIndex = 0;
                ReHit:
                using (var releaser = await LockAsync.LockAsync())
                {
                    _currentHitData = hitData;
                }
                var hitResult = await hitData.CheckHit(provider._pointLightButtonItems, provider._pointLightRelayItems, provider, isAdjoin);
                if (hitResult)
                {
                    chargeSpeedIndex++;
                    if (chargeSpeedIndex == chargeSpeed)
                    {
                        await provider._chargeLightRelayItems[hitDataIndex].Control();
                    }
                    else
                    {
                        goto ReHit;
                    }
                    hitDataIndex++;
                }
                else
                {
                    //await provider.PlayTextMusicFromFirstItem("踩踏失败。");
                    goto ReHit;
                }
            }
            var buttonItem = provider._pointLightButtonItems[0];
            UnregButtonCheckNotification(buttonItem.DeviceNumber, buttonItem.ServiceAddress, callback);
            using (var releaser = await LockAsync.LockAsync())
            {
                _currentHitData = null;
            }
        }

        public static async Task HitActionForBuffer7(List<HitData> hitDatas, HitMouseMachinePluginsProvider provider, int chargeSpeed = 1)
        {
            var callback = new TLAutoDevicePushCallback();
            RegButtonCheckNotification(provider._pointLightButtonItems, callback);
            callback.Notify += async (sen, eve) =>
                               {
                                   if (eve.IsError)
                                   {
                                       return;
                                   }
                                   var switchItem = eve.Data.ToObject<SwitchItem>();
                                   using (var releaser = await LockAsync.LockAsync())
                                   {
                                       if (_currentHitData != null)
                                       {
                                           await _currentHitData.Func(switchItem.SwitchNumber);
                                       }
                                   }
                               };
            for (var i = 0; i < hitDatas.Count; i++)
            {
                if (i % 2 == 0)
                {
                    var chargeSpeedIndex = 0;
                    ReHit:
                    using (var releaser = await LockAsync.LockAsync())
                    {
                        _currentHitData = hitDatas[i];
                    }
                    var hitResult = await hitDatas[i].CheckHit(provider._pointLightButtonItems, provider._pointLightRelayItems, provider);
                    if (hitResult)
                    {
                        chargeSpeedIndex++;
                        if (chargeSpeedIndex == chargeSpeed)
                        {
                            await provider._chargeLightRelayItems[i].Control();
                            await provider._chargeLightRelayItems[i + 1].Control();
                        }
                        else
                        {
                            goto ReHit;
                        }
                    }
                    else
                    {
                        //await provider.PlayTextMusicFromFirstItem("踩踏失败，重新来。");
                        goto ReHit;
                    }
                }
            }
            var buttonItem = provider._pointLightButtonItems[0];
            UnregButtonCheckNotification(buttonItem.DeviceNumber, buttonItem.ServiceAddress, callback);
            using (var releaser = await LockAsync.LockAsync())
            {
                _currentHitData = null;
            }
        }

        private static bool RegButtonCheckNotification(List<MachineButtonItem> checkLightButtonItems, TLAutoDevicePushCallback callback)
        {
            try
            {
                _wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, checkLightButtonItems[0].ServiceAddress);
                var result = _wcfService.Send(proxy =>
                                              {
                                                  proxy.RegistControlDeviceEx(HitMouseMachinePluginsProvider.SignKey + checkLightButtonItems[0].DeviceNumber,
                                                                              new ControlInfo
                                                                              {
                                                                                  ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                  Data = new PLCControlServiceData
                                                                                         {
                                                                                             ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                             DeviceNumber = checkLightButtonItems[0].DeviceNumber,
                                                                                             PortSignName = checkLightButtonItems[0].SignName
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

        private static void UnregButtonCheckNotification(int deviceNumber, string serviceAddress, TLAutoDevicePushCallback callback)
        {
            try
            {
                _wcfService.Close();
                var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, serviceAddress);
                service.Send(proxy => { proxy.UnRegistControlDeviceEx(HitMouseMachinePluginsProvider.SignKey + deviceNumber, CommonConfigHelper.PLCServiceKey); });
                service.Close();
            }
            catch (Exception ex) { }
        }
    }
}