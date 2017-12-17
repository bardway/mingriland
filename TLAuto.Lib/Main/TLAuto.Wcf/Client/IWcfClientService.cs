// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;

using TLAuto.Wcf.Client.Events;
#endregion

namespace TLAuto.Wcf.Client
{
    public interface IWcfClientService<TProxy>
    {
        bool Send(Action<TProxy> action);

        TReturn Send<TReturn>(Func<TProxy, TReturn> func);

        Task<TReturn> SendAsync<TReturn>(Func<TProxy, Task<TReturn>> funcAsync);

        event EventHandler<WcfClientServiceErrorEventArgs> CommunicationError;

        event EventHandler<WcfClientServiceErrorEventArgs> TimeoutError;

        event EventHandler<WcfClientServiceErrorEventArgs> Error;
    }
}