// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.Device.Contracts;
#endregion

namespace TLAuto.Machine.Plugins.Core.AsyncTask
{
    public class TLAutoDevicePushCallback : ITLAutoDevicePushCallback
    {
        public void NotifyMessage(WcfResultInfo wcfResutlInfo)
        {
            OnNotify(wcfResutlInfo);
        }

        public event EventHandler<WcfResultInfo> Notify;

        protected virtual void OnNotify(WcfResultInfo e)
        {
            Notify?.Invoke(this, e);
        }
    }
}