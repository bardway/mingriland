// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Machine.Plugins.Cooking.Models.Enums;
using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.AsyncTask;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Wcf.Client;

using Timer = System.Timers.Timer;
#endregion

namespace TLAuto.Machine.Plugins.Cooking.Models
{
    public class CookingData
    {
        private readonly Timer _buttonTimer = new Timer(500) {AutoReset = false};
        private readonly List<MachineButtonItem> _foodButtonItems;
        private readonly List<int> _foodValues = new List<int>();
        private readonly Timer _gameTimer = new Timer(1000);
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        private readonly MachineButtonItem _speedButtonItem;
        private readonly Timer _speedMusicTimer = new Timer(8000) {AutoReset = false};
        private readonly Timer _speedTimer = new Timer(500) {AutoReset = false};
        private bool _isCompleted;
        private CookingMachinePluginsProvider _provider;
        private volatile int _teperatureValue = 100;
        private volatile int _timeDelay = 30;

        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService1;
        private NetTcpDuplexWcfClientService<ITLAutoDevicePushService> _wcfService2;

        public CookingData(string[] foodNums, string[] temperatureNums, string delay, MachineButtonItem speedButtonItem, List<MachineButtonItem> foodButtonItems)
        {
            foreach (var foodNum in foodNums)
            {
                _foodValues.Add(0);
                FoodNums.Add(foodNum.ToInt32());
            }
            Temperature = new Tuple<int, int>(temperatureNums[0].ToInt32(), temperatureNums[1].ToInt32());
            Delay = delay.ToInt32();
            _gameTimer.Elapsed += GameTimer_Elapsed;
            _speedTimer.Elapsed += SpeedTimer_Elapsed;
            _speedMusicTimer.Elapsed += SpeedMusicTimer_Elapsed;
            _buttonTimer.Elapsed += ButtonTimer_Elapsed;
            _speedButtonItem = speedButtonItem;
            _foodButtonItems = foodButtonItems;
        }

        public List<int> FoodNums { get; } = new List<int>();

        public Tuple<int, int> Temperature { get; }

        public int Delay { get; }

        private void SpeedMusicTimer_Elapsed(object sender, ElapsedEventArgs e) { }

        private void ButtonTimer_Elapsed(object sender, ElapsedEventArgs e) { }

        private void SpeedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_teperatureValue <= 92)
            {
                SetTemperatureValue(_teperatureValue + 8);
            }
            else
            {
                SetTemperatureValue(100);
            }
        }

        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isCompleted)
            {
                SetTimeDelay(_timeDelay - 1);
                SetTemperatureValue(_teperatureValue - 6);
                if ((_teperatureValue >= Temperature.Item1) && (_teperatureValue <= Temperature.Item2))
                {
                    _isCompleted = !_foodValues.Where((t, i) => t != FoodNums[i]).Any();
                    if (_isCompleted)
                    {
                        _manualResetEvent.Set();
                    }
                }
            }
        }

        private void SetTemperatureValue(int value)
        {
            _teperatureValue = value;
            if (_teperatureValue < 0)
            {
                _teperatureValue = 0;
            }
            else
            {
                if (_teperatureValue > 90)
                {
                    if (!_speedMusicTimer.Enabled)
                    {
                        _speedMusicTimer.Start();
                        _provider.PlayMusic0(CookingMachinePluginsProvider.SignKey, "line2.wav");
                    }
                }
            }
            ArduinoHelper.ShowLed3(_teperatureValue, Led3Type.Temperature);
        }

        private void SetTimeDelay(int value)
        {
            _timeDelay = value;
            ArduinoHelper.ShowLed3(value, Led3Type.Time);
        }

        public bool Excute(CookingMachinePluginsProvider provider)
        {
            _provider = provider;
            SetTemperatureValue(90);
            SetTimeDelay(Delay);
            for (var i = 0; i < _foodValues.Count; i++)
            {
                _foodValues[i] = 0;
                ArduinoHelper.ShowLed2(0, i);
            }
            var speedCallback = new TLAutoDevicePushCallback();
            var foodCallback = new TLAutoDevicePushCallback();
            var result1 = RegButtonCheckNotification(_wcfService1, _speedButtonItem, speedCallback);
            var result2 = RegButtonCheckNotification(_wcfService2, _foodButtonItems[0], foodCallback);
            _gameTimer.Start();
            _manualResetEvent.Reset();
            if (result1 && result2)
            {
                _manualResetEvent.WaitOne(Delay * 1000);
                _gameTimer.Stop();
            }
            UnregButtonCheckNotification(_wcfService1, _speedButtonItem.DeviceNumber, speedCallback);
            UnregButtonCheckNotification(_wcfService2, _foodButtonItems[0].DeviceNumber, foodCallback);
            return _isCompleted;
        }

        private bool RegButtonCheckNotification(NetTcpDuplexWcfClientService<ITLAutoDevicePushService> wcfService, MachineButtonItem buttonItem, TLAutoDevicePushCallback callback)
        {
            callback.Notify += (s, e) => { Callback_Notify(s, e, buttonItem.DeviceNumber); };
            try
            {
                wcfService = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, buttonItem.ServiceAddress);
                var result = wcfService.Send(proxy =>
                                             {
                                                 proxy.RegistControlDeviceEx(CookingMachinePluginsProvider.SignKey + buttonItem.DeviceNumber,
                                                                             new ControlInfo
                                                                             {
                                                                                 ServiceKey = CommonConfigHelper.PLCServiceKey,
                                                                                 Data = new PLCControlServiceData
                                                                                        {
                                                                                            ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload,
                                                                                            DeviceNumber = buttonItem.DeviceNumber,
                                                                                            PortSignName = buttonItem.SignName
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

        private void UnregButtonCheckNotification(NetTcpDuplexWcfClientService<ITLAutoDevicePushService> wcfService, int deviceNumber, TLAutoDevicePushCallback callback)
        {
            try
            {
                wcfService.Close();
                var service = new NetTcpDuplexWcfClientService<ITLAutoDevicePushService>(callback, _speedButtonItem.ServiceAddress);
                service.Send(proxy => { proxy.UnRegistControlDeviceEx(CookingMachinePluginsProvider.SignKey + deviceNumber, CommonConfigHelper.PLCServiceKey); });
                service.Close();
            }
            catch (Exception ex) { }
        }

        private void Callback_Notify(object sender, WcfResultInfo e, int deviceNumber)
        {
            if (!_gameTimer.Enabled)
            {
                return;
            }
            if (!e.IsError)
            {
                var switchItem = e.Data.ToObject<SwitchItem>();
                if ((switchItem.SwitchNumber == _speedButtonItem.Number) && (deviceNumber == _speedButtonItem.DeviceNumber))
                {
                    if (!_speedTimer.Enabled)
                    {
                        _speedTimer.Start();
                    }
                }
                else
                {
                    var foodButtonItem = _foodButtonItems.FirstOrDefault(s => (s.Number == switchItem.SwitchNumber) && (s.DeviceNumber == deviceNumber));
                    if (foodButtonItem != null)
                    {
                        //Console.WriteLine(foodButtonItem.Number);
                        if (!_buttonTimer.Enabled)
                        {
                            _buttonTimer.Start();
                            var buttonIndex = _foodButtonItems.IndexOf(foodButtonItem);
                            var foodValue = _foodValues[buttonIndex];
                            foodValue = foodValue == 10 ? 0 : foodValue + 1;
                            _foodValues[buttonIndex] = foodValue;
                            ArduinoHelper.ShowLed2(foodValue, buttonIndex);
                        }
                    }
                }
            }
        }
    }
}