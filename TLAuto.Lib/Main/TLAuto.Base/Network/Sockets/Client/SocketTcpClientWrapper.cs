// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using TLAuto.Base.Network.Events;
#endregion

namespace TLAuto.Base.Network.Sockets.Client
{
    public sealed class SocketTcpClientWrapper : INetworkBaseWrapper
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly int _connectTimeout;
        private readonly object _lockObjIsConnectSuccess = new object();
        private readonly int _receiveTimeout;
        private readonly IPEndPoint _remotePoint;
        private readonly int _sendTimeout;
        private readonly ManualResetEvent _timeoutObject = new ManualResetEvent(false);
        private Socket _socket;

        public SocketTcpClientWrapper
        (
            string remoteIp,
            int remotePort,
            int connectTimeout = 10000,
            int sendTimeout = 3000,
            int receiveTimeout = 3000)
        {
            _remotePoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
            _connectTimeout = connectTimeout;
            _sendTimeout = sendTimeout;
            _receiveTimeout = receiveTimeout;
        }

        public bool Connect()
        {
            try
            {
                if (_socket == null)
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
            catch (SocketException se)
            {
                OnError(new NetworkBaseErrorEventArgs(se.Message, se));
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Disconnect(true);
                    IsConnected = false;
                    _socket.Close();
                    _socket = null;
                }
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs("Close Socket Error.", ex));
            }
        }

        public void Dispose()
        {
            Close();
        }

        public bool Send(byte[] bytes)
        {
            var result = false;
            try
            {
                var n = _socket.Send(bytes);
                result = n >= 1;
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
            }
            return result;
        }

        public bool IsConnected { private set; get; }

        ~SocketTcpClientWrapper()
        {
            Dispose();
        }

        private bool CreateSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                      {
                          SendTimeout = _sendTimeout,
                          ReceiveTimeout = _receiveTimeout
                      };
            SocketHelper.SetKeepAlive(_socket);
            return CreateConnect();
        }

        private bool CreateConnect()
        {
            _timeoutObject.Reset();
            try
            {
                _socket.BeginConnect(_remotePoint, ConnectedCallback, _socket);
            }
            catch (Exception ex)
            {
                OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
                return false;
            }
            if (_timeoutObject.WaitOne(_connectTimeout, false)) //直到timeout，或者TimeoutObject.set()
            {
                return IsConnected;
            }
            OnError(new NetworkBaseErrorEventArgs("Connect Timeout.", new TimeoutException()));
            return false;
        }

        private void ConnectedCallback(IAsyncResult iar)
        {
            lock (_lockObjIsConnectSuccess)
            {
                var client = (Socket)iar.AsyncState;
                try
                {
                    client.EndConnect(iar);
                    IsConnected = true;
                    StartReceiveData();
                    OnConnected();
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    Close();
                    OnError(new NetworkBaseErrorEventArgs(ex.Message, ex));
                    OnDisconnected(new NetworkDisconnectedEventArgs("Socket Disconnected", ex));
                }
                finally
                {
                    _timeoutObject.Set();
                }
            }
        }

        private void StartReceiveData()
        {
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceiveCallback, _socket);
        }

        private void OnReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var peerSock = (Socket)ar.AsyncState;
                var bytesRead = peerSock.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var tmp = new byte[bytesRead];
                    Array.ConstrainedCopy(_buffer, 0, tmp, 0, bytesRead);
                    OnDataReceived(new NetworkBaseDataReceivedEventArgs(tmp));
                }
                else //对端gracefully关闭一个连接
                {
                    if (_socket.Connected) //上次socket的状态
                    {
                        Close();
                        OnDisconnected(new NetworkDisconnectedEventArgs("Socket Disconnected，对端gracefully关闭一个连接", null));
                        return;
                    }
                }
                _buffer.Initialize();
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceiveCallback, _socket);
            }
            catch (Exception ex)
            {
                Close();
                OnDisconnected(new NetworkDisconnectedEventArgs("Socket Disconnected", ex));
            }
        }

        #region Events
        public event EventHandler Connected;

        private void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<NetworkBaseDataReceivedEventArgs> DataReceived;

        private void OnDataReceived(NetworkBaseDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public event EventHandler<NetworkBaseErrorEventArgs> Error;

        private void OnError(NetworkBaseErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler<NetworkDisconnectedEventArgs> Disconnected;

        private void OnDisconnected(NetworkDisconnectedEventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }
        #endregion
    }
}