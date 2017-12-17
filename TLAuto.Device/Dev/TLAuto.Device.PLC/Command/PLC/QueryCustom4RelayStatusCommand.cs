// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;

using TLAuto.Base.Extensions;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.Command.PLC
{
    public class QueryCustom4RelayStatusCommand : PLCDeviceCommand<IEnumerable<RelayItem>>
    {
        public QueryCustom4RelayStatusCommand(int deviceNumber, int itemCount) : base(deviceNumber, itemCount) { }

        /// <summary>
        /// 0代表设备号
        /// </summary>
        protected override string SendCommand => "AA 5A {0} FC 00 FF";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;

        protected override int ResultByteLength => 6;

        protected override bool HasSendCRC => false;

        protected override bool CheckReveicedDataHeader(byte[] bytes, out int headerLength)
        {
            headerLength = 1;
            return bytes[0] == 170;
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if ((bytes[1] == 90) && (bytes[2].ToInt32() == DeviceNumber) && (bytes[3] == 252))
            {
                return true;
            }
            return false;
        }

        protected override IEnumerable<RelayItem> GetResult(byte[] bytes)
        {
            var relayItems = new List<RelayItem>();
            var one = bytes[4].ToBin();
            var two = bytes[5].ToBin();
            for (var i = 0; i < 8; i++)
            {
                relayItems.Add(new RelayItem(i + 1, (RelayStatus)Enum.Parse(typeof(RelayStatus), one[i].ToString())));
            }
            for (var i = 0; i < 8; i++)
            {
                relayItems.Add(new RelayItem(i + 9, (RelayStatus)Enum.Parse(typeof(RelayStatus), two[i].ToString())));
            }
            return relayItems;
        }
    }
}