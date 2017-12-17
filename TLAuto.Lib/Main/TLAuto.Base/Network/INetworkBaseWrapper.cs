// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.Base.Network.Events;
#endregion

namespace TLAuto.Base.Network
{
    public interface INetworkBaseWrapper : IDisposable
    {
        bool IsConnected { get; }

        bool Connect();

        void Close();

        bool Send(byte[] data);

        event EventHandler Connected;

        event EventHandler<NetworkBaseDataReceivedEventArgs> DataReceived;

        event EventHandler<NetworkBaseErrorEventArgs> Error;

        event EventHandler<NetworkDisconnectedEventArgs> Disconnected;
    }
}