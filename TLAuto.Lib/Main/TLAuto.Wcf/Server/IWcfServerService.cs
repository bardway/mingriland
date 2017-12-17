// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;

using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Wcf.Server
{
    public interface IWcfServerService
    {
        Task<bool> StartWcfService(string serviceAddress, Type typeClass, params Type[] typeInterfaces);

        void StopWcfService();

        event EventHandler<WcfServerServiceErrorMessageEventArgs> Error;

        event EventHandler<WcfServerServiceMessageEventArgs> Closed;

        event EventHandler Opened;
    }
}