/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;

namespace TLAuto.Device.Ports.Events
{
    public class TLSerialErrorReceivedEventArgs : EventArgs
    {
        public TLSerialErrorReceivedEventArgs(string errorMsg)
        {
            ErrorMsg = errorMsg;
        }

        public string ErrorMsg { get; private set; }
    }
}
