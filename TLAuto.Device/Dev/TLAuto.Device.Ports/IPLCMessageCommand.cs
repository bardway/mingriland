/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLAuto.Device.Ports
{
    public interface IPLCMessageCommand<T>
    {
        byte[] GetSendData();


    }
}