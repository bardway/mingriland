// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Net;
using System.Net.Sockets;

using TLAuto.Base.Network;
using TLAuto.Base.Network.Events;
#endregion

namespace TLAuto.Device.IoT.View.Sockets
{
    public class ScoketUdpServerWarrper : INetworkBaseWrapper
    {
        private IPEndPoint _serverPoint;
        private UdpClient _udpClient;

        public ScoketUdpServerWarrper(int localPort)
        {
            _serverPoint = new IPEndPoint(IPAddress.Any, localPort);
        }

        public bool Connect()
        {
            try
            {
                if (_udpClient == null)
                {
                    return CreateSocket();
                }
                if (IsConnected)
                {
                    return true;
                }
                //已创建套接字，但未connected
                return CreateConnect();
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (_udpClient != null)
                {
                    _udpClient.Close();
                    _udpClient.Dispose();
                    IsConnected = false;
                    _udpClient = null;
                }
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs("Close Socket Error.", ex));
            }
        }

        public bool Send(byte[] data)
        {
            return true;
        }

        public void Dispose()
        {
            Close();
        }

        public bool IsConnected { private set; get; }

        ~ScoketUdpServerWarrper()
        {
            Dispose();
        }

        private bool CreateSocket()
        {
            _udpClient = new UdpClient(_serverPoint);
            return CreateConnect();
        }

        private bool CreateConnect()
        {
            IsConnected = true;
            try
            {
                _udpClient.BeginReceive(CallbackAccept, null);
                return true;
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
            }
            return false;
        }

        private void CallbackAccept(IAsyncResult ar)
        {
            try
            {
                var receiveBytes = _udpClient.EndReceive(ar, ref _serverPoint);
                OnDataReceived(new NetworkBaseDataReceivedEventArgs(receiveBytes));
                _udpClient.BeginReceive(CallbackAccept, null);
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
            }
        }

        #region Events
        public event EventHandler Connected;

        public event EventHandler<NetworkBaseDataReceivedEventArgs> DataReceived;

        public event EventHandler<NetworkBaseErrorEventArgs> Error;

        public event EventHandler<NetworkDisconnectedEventArgs> Disconnected;

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDataReceived(NetworkBaseDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        protected virtual void OnError(NetworkBaseErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnDisconnected(NetworkDisconnectedEventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }
        #endregion
    }
}