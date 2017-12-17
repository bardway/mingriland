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
    public class ControlRelayStatusCustom4Command : PLCDeviceCommand<bool>
    {
        private readonly int _relayNumber;
        private readonly RelayStatus _relayStatus;

        public ControlRelayStatusCustom4Command(int deviceNumber, int relayNumber, RelayStatus relayStatus, int itemCount) :
            base(deviceNumber, itemCount)
        {
            _relayNumber = relayNumber;
            _relayStatus = relayStatus;
        }

        /// <summary>
        /// 0代表设备号，1代表继电器号，2代表闭合还是常开
        /// </summary>
        protected override string SendCommand => "AA 5A {0} {1} 00 FF";

        public override bool HasSendData => true;

        public override bool HasReceivedData => false;

        protected override bool HasSendCRC => false;

        protected override IEnumerable<object> GetReplaceCommandParams()
        {
            var commandParams = new List<object>
                                {
                                    $"{(_relayNumber - 1).ToHexWithoutPadLeft().ToUpper()}{(int)_relayStatus}"
                                };
            return commandParams;
        }
    }
}