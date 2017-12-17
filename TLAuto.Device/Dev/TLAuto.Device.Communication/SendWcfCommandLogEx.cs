/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.ServiceModel;
using TLAuto.Log;

namespace TLAuto.Device.Communication
{
    public class SendWcfCommandLogEx<TProxy, TReturn> : SendWcfCommand<TProxy, TReturn>
    {
        private readonly LogWraper _log;

        public SendWcfCommandLogEx(string serviceAddress, LogWraper logWraper)
            : base(serviceAddress)
        {
            _log = logWraper;
        }

        protected override void LogCommunicationException(CommunicationException ce)
        {
            _log.Critical(ce.Message, ce);
        }

        protected override void LogTimeoutException(TimeoutException te)
        {
            _log.Critical(te.Message, te);
        }

        protected override void LogException(Exception ce)
        {
            _log.Critical(ce.Message, ce);
        }
    }
}