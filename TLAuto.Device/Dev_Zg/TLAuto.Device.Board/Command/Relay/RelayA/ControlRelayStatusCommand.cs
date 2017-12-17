/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLAuto.Base.Extensions;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC.Command.Relay.RelayA
{
    public class ControlRelayStatusCommand : BoardDeviceCommand<bool>
    {
        private readonly int _relayNumber;
        private readonly RelayStatus _relayStatus;

        public ControlRelayStatusCommand(int deviceNumber, int relayNumber, RelayStatus relayStatus, int itemCount)
            : base(deviceNumber, itemCount)
        {
            _relayNumber = relayNumber;
            _relayStatus = relayStatus;
        }

        protected override IEnumerable<object> GetReplaceCommandParams()
        {
            var commandParams = new List<object>
            {
                 _relayNumber.ToHex(),
                 ((int)_relayStatus).ToHex()
            };
            return commandParams;
        }

        /// <summary>
        /// 0代表设备号，1代表继电器号，2代表闭合还是常开
        /// </summary>
        protected override string SendCommand => "AA BB {0} {1} {2}";

        public override bool HasSendData => true;

        public override bool HasReceivedData => false;
    }
}
