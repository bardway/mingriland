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
    public class AutoWcfServiceErrorMessageEventArgs : AutoWcfServiceMessageEventArgs
    {
        public AutoWcfServiceErrorMessageEventArgs(string msg, Exception ex) 
            : base(msg)
        {
            Ex = ex;
        }

        public Exception Ex { private set; get; }
    }
}