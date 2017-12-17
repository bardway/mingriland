// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.Controls.NavFrame;
using TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings;
using TLAuto.Device.Events;
using TLAuto.Device.PLC.Command.PLC;
using TLAuto.Device.PLC.ServiceData;
using TLAuto.Device.PLC.View.Config;
using TLAuto.Device.PLC.View.NavPages;
using TLAuto.Device.PLC.View.ViewModels;
#endregion

namespace TLAuto.Device.PLC.View.Models
{
    public class PLCSerialPortInfo : ViewModelBase
    {
        private readonly ConcurrentQueue<byte[]> _cacheData = new ConcurrentQueue<byte[]>();
        private readonly ConcurrentDictionary<string, Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback>> _dicCallbacks = new ConcurrentDictionary<string, Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback>>();

        private bool _isOpenSerialPort;
        private volatile bool _isWhileAnalyseData;

        private string _portName;

        private string _portSignName;

        public PLCSerialPortInfo(SerialPortInfo serialPortInfo)
        {
            UpdateInfo(serialPortInfo);
            InitOpenSerialPortCommand();
            InitEditCommand();
            InitDeleteCommand();
            TLSerialPort.DataReceived += TLSerialPort_DataReceived;
            Task.Factory.StartNew(AnalyseDatas);
        }

        internal TLAutoDeviceSerialPort TLSerialPort { get; } = new TLAutoDeviceSerialPort(ConfigHelper.SerialPortSendTime);

        public string PortName
        {
            set
            {
                _portName = value;
                RaisePropertyChanged();
            }
            get => _portName;
        }

        public string PortSignName
        {
            set
            {
                _portSignName = value;
                RaisePropertyChanged();
            }
            get => _portSignName;
        }

        public bool IsOpenSerialPort
        {
            private set
            {
                _isOpenSerialPort = value;
                RaisePropertyChanged();
            }
            get => _isOpenSerialPort;
        }

        public MainViewModel Parent => PLCDeviceService.GetMainViewModel();

        public ObservableCollection<PLCDeviceInfo> PLCInfos { get; } = new ObservableCollection<PLCDeviceInfo>();

        private void AnalyseDatas()
        {
            while (!_isWhileAnalyseData)
            {
                Thread.Sleep(50);
                var caches = new List<byte[]>();
                while (!_cacheData.IsEmpty)
                {
                    byte[] data;
                    if (_cacheData.TryDequeue(out data))
                    {
                        caches.Add(data);
                    }
                }
                if (!_dicCallbacks.IsEmpty)
                {
                    foreach (var key in _dicCallbacks.Keys)
                    {
                        Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback> callBackTuple;
                        if (_dicCallbacks.TryGetValue(key, out callBackTuple))
                        {
                            foreach (var data in caches)
                            {
                                IList<SwitchItem> switchItems;
                                if (callBackTuple.Item1.ParseReceivedData(data, out switchItems))
                                {
                                    try
                                    {
                                        foreach (var switchItem in switchItems)
                                        {
                                            callBackTuple.Item2.NotifyMessage(new WcfResultInfo
                                                                              {
                                                                                  Data = switchItem.ToBytes()
                                                                              });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var plcInfo = PLCInfos.FirstOrDefault(s => s.DeviceNumber == callBackTuple.Item1.DeviceNumber);
                                        plcInfo?._logWraper.Critical("开关检测回调出错", ex);
                                        Remove(key);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void TLSerialPort_DataReceived(object sender, TLAutoDeviceDataReceivedEventArgs e)
        {
            _cacheData.Enqueue(e.Bytes);
        }

        public override void Cleanup()
        {
            _isWhileAnalyseData = true;
            foreach (var plcInfo in PLCInfos)
            {
                plcInfo.Cleanup();
            }
            PLCInfos.Clear();
            var deviceSettings = PLCDeviceSettings.GetPLCDeviceSettings();
            deviceSettings.RemoveSerialPortSettings(PortName);
            ClosePort();
            TLSerialPort.DataReceived -= TLSerialPort_DataReceived;
            base.Cleanup();
        }

        private void UpdateInfo(SerialPortInfo serialPortInfo)
        {
            PortName = serialPortInfo.PortName;
            PortSignName = serialPortInfo.PortSignName;
            foreach (var plcDeviceInfo in PLCInfos)
            {
                plcDeviceInfo.UpdatePortName(PortName);
            }
        }

        public void AddPLCDeviceInfoWithSave(PLCInfo projectorInfo)
        {
            PLCInfos.Add(new PLCDeviceInfo(projectorInfo, PortName));
            var serialPortSettings = PLCDeviceSettings.GetSerialPortSettings(PortName);
            serialPortSettings.AddDetailDevice(projectorInfo);
            PLCDeviceService.SaveSettings();
        }

        public void Add(string key, PLCControlServiceData serviceData, ITLAutoDevicePushCallback callBack)
        {
            if (serviceData.ControlPLCType == ControlPLCType.QueryDiaitalSwitchWithAutoUpload)
            {
                var plcInfo = PLCInfos.FirstOrDefault(s => s.DeviceNumber == serviceData.DeviceNumber);
                if (plcInfo != null)
                {
                    switch (plcInfo.PLCDeviceType)
                    {
                        case PLCDeviceType.Custom1:
                        case PLCDeviceType.Custom2:
                        case PLCDeviceType.Custom3:
                        {
                            var autoUploadCommand = new AutoUploadSwitchStatusExCommand(serviceData.DeviceNumber, plcInfo.DigitalSwitchNumber);
                            if (_dicCallbacks.ContainsKey(key))
                            {
                                Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback> removeInfo;
                                _dicCallbacks.TryRemove(key, out removeInfo);
                            }
                            _dicCallbacks.TryAdd(key, new Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback>(autoUploadCommand, callBack));
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void Remove(string key)
        {
            Tuple<AutoUploadSwitchStatusExCommand, ITLAutoDevicePushCallback> callBackTuple;
            _dicCallbacks.TryRemove(key, out callBackTuple);
        }

        internal Tuple<SerialPortInfo, bool> GetCurrentUsedSerialPortInfo()
        {
            return new Tuple<SerialPortInfo, bool>(PLCDeviceSettings.GetSerialPortInfo(PortName), IsOpenSerialPort);
        }

        private void ClosePort()
        {
            try
            {
                if (IsOpenSerialPort)
                {
                    TLSerialPort.Close();
                }
                IsOpenSerialPort = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenPort()
        {
            try
            {
                if (!_isOpenSerialPort)
                {
                    var serialPortInfo = PLCDeviceSettings.GetSerialPortInfo(PortName);
                    TLSerialPort.Open(serialPortInfo.PortName, serialPortInfo.BaudRates, serialPortInfo.Parity, serialPortInfo.DataBits, serialPortInfo.StopBits);
                    IsOpenSerialPort = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Events Mvvmbindings
        private void InitOpenSerialPortCommand()
        {
            OpenSerialPortCommand = new RelayCommand(() =>
                                                     {
                                                         if (IsOpenSerialPort)
                                                         {
                                                             ClosePort();
                                                         }
                                                         else
                                                         {
                                                             OpenPort();
                                                         }
                                                     });
        }

        public RelayCommand OpenSerialPortCommand { private set; get; }

        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var serialPortSettingsInfo = new SerialPortSettingsInfo(Parent.GetAllUsedSerialPortInfo(), true, PortName);
                                               var navFrameInfos = new List<NavFrameInfo>
                                                                   {
                                                                       new NavFrameInfo(serialPortSettingsInfo)
                                                                   };
                                               var nav = new NavFrameWindow(navFrameInfos, "编辑设备信息");
                                               if (nav.ShowDialog() == true)
                                               {
                                                   var oldSerialPortSettings = PLCDeviceSettings.GetSerialPortSettings(PortName);
                                                   oldSerialPortSettings.UpdateInfo(serialPortSettingsInfo.Current.Item1);
                                                   UpdateInfo(serialPortSettingsInfo.Current.Item1);
                                                   PLCDeviceService.SaveSettings();
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

        private void InitDeleteCommand()
        {
            DeleteCommand = new RelayCommand(() =>
                                             {
                                                 var dialog = new RemoveItemsView("删除设备", PLCInfos.Select(s => s.HeaderName).ToList());
                                                 if (dialog.ShowDialog() == true)
                                                 {
                                                     var removeHeaderNames = dialog.SelectedItems;
                                                     foreach (var headerName in removeHeaderNames)
                                                     {
                                                         var removeInfo = PLCInfos.FirstOrDefault(s => s.HeaderName == headerName);
                                                         if (removeInfo != null)
                                                         {
                                                             removeInfo.Cleanup();
                                                             PLCInfos.Remove(removeInfo);
                                                         }
                                                     }
                                                     PLCDeviceService.SaveSettings();
                                                 }
                                             });
        }

        public RelayCommand DeleteCommand { private set; get; }
        #endregion
    }
}