/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TLAuto.Base.Async;

namespace TLAuto.Device.Ports
{
    public class TLSerialPortMessageAsync<T> : AsyncTaskBase<T>
    {
        public TLSerialPortMessageAsync(TLSerialPort tlSerialPort, CancellationToken? cancelToken, int timeOutMs = 4000) : 
            base(cancelToken, timeOutMs)
        {

        }

        protected override void Invoke()
        {
            throw new NotImplementedException();
        }

        protected override void Exception(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}