/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLAuto.Device.Communication.Events
{
    public class AutoWcfServiceMessageEventArgs : EventArgs
    {
        public AutoWcfServiceMessageEventArgs(string msg)
        {
            Msg = msg;
        }

        public string Msg { private set; get; }
    }
}