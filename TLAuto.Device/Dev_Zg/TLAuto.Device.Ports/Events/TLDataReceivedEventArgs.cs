/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLAuto.Device.Ports.Events
{
    public class TLDataReceivedEventArgs : EventArgs
    {
        public TLDataReceivedEventArgs(byte[] data)
        {
            Datas = data;
        }

        public byte[] Datas { private set; get; }
    }
}