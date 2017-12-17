// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;

using TLAuto.Wcf.Server.Events;
#endregion

namespace TLAuto.Wcf
{
    public interface IWcfServerWrapper
    {
        Task<bool> StartWcfService(string serviceAddress);

        void StopWcfService();

        event EventHandler<WcfServerServiceErrorMessageEventArgs> Error;

        event EventHandler<WcfServerServiceMessageEventArgs> Close;

        event EventHandler Opened;

        event EventHandler<WcfServerServiceMessageEventArgs> Info;
    }
}