// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Network;
using TLAuto.Base.Network.Events;
using TLAuto.Base.Network.Sockets.Client;
using TLAuto.Device.Controls.Dialogs;
using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.IoT.View.Config;
using TLAuto.Device.IoT.View.Models.Enums;
using TLAuto.Device.IoT.View.Sockets;
using TLAuto.Device.IoT.View.Views.Dialogs;
#endregion

namespace TLAuto.Device.IoT.View.Models
{
    public class IoTSocketInfo : ViewModelBase
    {
        private IoTSocketType _ioTSocketType;

        private string _ip;

        private bool _isOpenScoket;

        private int _port;

        private string _signName;

        internal INetworkBaseWrapper SocketTcpWrapper;
        //private readonly ConcurrentDictionary<string, Tuple<IoTControlServiceData, ITLAutoDevicePushCallback>> _dicCallbacks = new ConcurrentDictionary<string, Tuple<IoTControlServiceData, ITLAutoDevicePushCallback>>();

        public IoTSocketInfo(string ip, int port, string signName, IoTSocketType iotSocketType)
        {
            UpdateInfo(ip, port, signName, iotSocketType);
            InitOpenIpCommand();
            InitEditCommand();
            InitDeleteCommand();
        }

        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                RaisePropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                RaisePropertyChanged();
            }
        }

        public string SignName
        {
            get => _signName;
            set
            {
                _signName = value;
                RaisePropertyChanged();
            }
        }

        public IoTSocketType IoTSocketType
        {
            set
            {
                _ioTSocketType = value;
                RaisePropertyChanged();
            }
            get => _ioTSocketType;
        }

        public bool IsOpenScoket
        {
            private set
            {
                _isOpenScoket = value;
                RaisePropertyChanged();
            }
            get => _isOpenScoket;
        }

        public ObservableCollection<IoTDeviceInfo> IoTInfos { get; } = new ObservableCollection<IoTDeviceInfo>();

        public override void Cleanup()
        {
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                ioTDeviceInfo.Cleanup();
            }
            IoTInfos.Clear();
            var deviceSettings = IoTDeviceSettings.GetIoTDeviceSettings();
            deviceSettings.RemoveSocketSettings(SignName);
            CloseSocket();
            base.Cleanup();
        }

        public void UpdateInfo(string ip, int port, string signName, IoTSocketType iotSocketType)
        {
            Ip = ip;
            Port = port;
            SignName = signName;
            IoTSocketType = iotSocketType;
            foreach (var iotInfo in IoTInfos)
            {
                iotInfo.UpdatePortName(signName);
            }
        }

        public void AddIoTDeviceInfoWithSave(int deviceNumber, string deviceHeader)
        {
            IoTInfos.Add(new IoTDeviceInfo(deviceNumber, deviceHeader, SignName));
            var socketSettings = IoTDeviceSettings.GetIoTDeviceSettings(SignName);
            socketSettings.AddDetailDevice(deviceNumber, deviceHeader);
            IoTDeviceService.SaveSettings();
        }

        private void CloseSocket()
        {
            try
            {
                if (SocketTcpWrapper != null)
                {
                    SocketTcpWrapper.Connected -= SocketTcpWrapper_Connected;
                    SocketTcpWrapper.Disconnected -= SocketTcpWrapper_Disconnected;
                    SocketTcpWrapper.Error -= SocketTcpWrapper_Error;
                    SocketTcpWrapper.DataReceived -= SocketTcpWrapper_DataReceived;
                    SocketTcpWrapper.Dispose();
                    SocketTcpWrapper = null;
                    IsOpenScoket = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenSocket()
        {
            try
            {
                if (!IsOpenScoket)
                {
                    var socketSettings = IoTDeviceSettings.GetIoTDeviceSettings(SignName);
                    SocketTcpWrapper = IoTSocketType == IoTSocketType.Client ? (INetworkBaseWrapper)new SocketTcpClientWrapper(socketSettings.Ip, socketSettings.Port) : new ScoketUdpServerWarrper(socketSettings.Port);
                    SocketTcpWrapper.Connected += SocketTcpWrapper_Connected;
                    SocketTcpWrapper.Disconnected += SocketTcpWrapper_Disconnected;
                    SocketTcpWrapper.Error += SocketTcpWrapper_Error;
                    SocketTcpWrapper.DataReceived += SocketTcpWrapper_DataReceived;
                    IsOpenScoket = SocketTcpWrapper.Connect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SocketTcpWrapper_DataReceived(object sender, NetworkBaseDataReceivedEventArgs e)
        {
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                if (ioTDeviceInfo.IsShowServerReceivedInfo)
                {
                    var content = Encoding.Default.GetString(e.Data);
                    ioTDeviceInfo.WriteMsgWithLog(content, StatusNotificationType.NInfo);
                }
            }
            //if (!_dicCallbacks.IsEmpty)
            //{
            //    foreach (var key in _dicCallbacks.Keys)
            //    {
            //        Tuple<IoTControlServiceData, ITLAutoDevicePushCallback> callBackTuple;
            //        if (_dicCallbacks.TryGetValue(key, out callBackTuple))
            //        {
            //            try
            //            {
            //                callBackTuple.Item2.NotifyMessage(new WcfResultInfo
            //                {
            //                    Data = e.Data
            //                });
            //            }
            //            catch (Exception ex)
            //            {
            //                var info = IoTInfos.FirstOrDefault(s => s.DeviceNumber == callBackTuple.Item1.DeviceNumber);
            //                info?._logWraper.Critical("Arduino检测回调出错", ex);
            //                Remove(key);
            //            }
            //        }
            //    }
            //}
        }

        //public void Add(string key, IoTControlServiceData serviceData, ITLAutoDevicePushCallback callBack)
        //{
        //    var iotInfo = IoTInfos.FirstOrDefault(s => s.DeviceNumber == serviceData.DeviceNumber);
        //    if (iotInfo != null)
        //    {
        //        if (_dicCallbacks.ContainsKey(key))
        //        {
        //            Tuple<IoTControlServiceData, ITLAutoDevicePushCallback> removeInfo;
        //            _dicCallbacks.TryRemove(key, out removeInfo);
        //        }
        //        _dicCallbacks.TryAdd(key, new Tuple<IoTControlServiceData, ITLAutoDevicePushCallback>(serviceData, callBack));
        //    }
        //}

        //public void Remove(string key)
        //{
        //    Tuple<IoTControlServiceData, ITLAutoDevicePushCallback> callBackTuple;
        //    _dicCallbacks.TryRemove(key, out callBackTuple);
        //}

        private void SocketTcpWrapper_Connected(object sender, EventArgs e)
        {
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                ioTDeviceInfo.WriteMsgWithLog("连接成功。", StatusNotificationType.NInfo);
            }
        }

        private void SocketTcpWrapper_Error(object sender, NetworkBaseErrorEventArgs e)
        {
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                ioTDeviceInfo.WriteMsgWithLog(e.Msg + " 异常为:" + e.Ex, StatusNotificationType.Error);
            }
        }

        private async void SocketTcpWrapper_Disconnected(object sender, NetworkDisconnectedEventArgs e)
        {
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                ioTDeviceInfo.WriteMsgWithLog(e.Msg + " 异常为:" + e.Ex, StatusNotificationType.Error);
            }
            foreach (var ioTDeviceInfo in IoTInfos)
            {
                ioTDeviceInfo.WriteMsgWithLog("等待5S重新连接...", StatusNotificationType.NInfo);
            }
            await Task.Delay(5000);
            SocketTcpWrapper.Connect();
        }

        #region Events Mvvmbindings
        private void InitOpenIpCommand()
        {
            OpenIpCommand = new RelayCommand(() =>
                                             {
                                                 if (IsOpenScoket)
                                                 {
                                                     CloseSocket();
                                                 }
                                                 else
                                                 {
                                                     OpenSocket();
                                                 }
                                             });
        }

        public RelayCommand OpenIpCommand { private set; get; }

        private void InitEditCommand()
        {
            EditCommand = new RelayCommand(() =>
                                           {
                                               var editDialog = new EditIpAddressDialog(Ip, Port, SignName, 0, "不填", IoTSocketType);
                                               if (editDialog.ShowDialog() == true)
                                               {
                                                   var oldSocketSettings = IoTDeviceSettings.GetIoTDeviceSettings(SignName);
                                                   oldSocketSettings.UpdateInfo(editDialog.Ip, editDialog.Port, editDialog.SignName, editDialog.IoTScoketType);
                                                   UpdateInfo(editDialog.Ip, editDialog.Port, editDialog.SignName, editDialog.IoTScoketType);
                                                   IoTDeviceService.SaveSettings();
                                               }
                                           });
        }

        public RelayCommand EditCommand { private set; get; }

        private void InitDeleteCommand()
        {
            DeleteCommand = new RelayCommand(() =>
                                             {
                                                 var dialog = new RemoveItemsView("删除设备", IoTInfos.Select(s => s.DeviceHeader).ToList());
                                                 if (dialog.ShowDialog() == true)
                                                 {
                                                     var removeHeaderNames = dialog.SelectedItems;
                                                     foreach (var headerName in removeHeaderNames)
                                                     {
                                                         var removeInfo = IoTInfos.FirstOrDefault(s => s.DeviceHeader == headerName);
                                                         if (removeInfo != null)
                                                         {
                                                             removeInfo.Cleanup();
                                                             IoTInfos.Remove(removeInfo);
                                                         }
                                                     }
                                                     IoTDeviceService.SaveSettings();
                                                 }
                                             });
        }

        public RelayCommand DeleteCommand { private set; get; }
        #endregion
    }
}