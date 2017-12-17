// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using TLAuto.Base.Extensions;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.Command.PLC
{
    public class ControlRelayStatusCommand : PLCDeviceCommand<bool>
    {
        private readonly int _relayNumber;
        private readonly RelayStatus _relayStatus;

        public ControlRelayStatusCommand(int deviceNumber, int relayNumber, RelayStatus relayStatus, int itemCount, bool hasResult)
            : base(deviceNumber, itemCount)
        {
            _relayNumber = relayNumber;
            _relayStatus = relayStatus;
            HasReceivedData = hasResult;
        }

        /// <summary>
        /// 0代表设备号，1代表继电器号，2代表闭合还是常开
        /// </summary>
        protected override string SendCommand => "AA BB {0} {1} {2}";

        public override bool HasSendData => true;

        public override bool HasReceivedData { get; }

        protected override int ResultByteLength => 6;

        protected override IEnumerable<object> GetReplaceCommandParams()
        {
            var commandParams = new List<object>
                                {
                                    _relayNumber.ToHex(),
                                    ((int)_relayStatus).ToHex()
                                };
            return commandParams;
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if (!HasReceivedData)
            {
                return true;
            }
            if ((bytes[1] == 221) && (bytes[ResultByteLength - 1] == 255))
            {
                return true;
            }
            return base.CheckReveicedDataBody(bytes);
        }

        protected override bool GetResult(byte[] bytes)
        {
            return true;
        }
    }
}