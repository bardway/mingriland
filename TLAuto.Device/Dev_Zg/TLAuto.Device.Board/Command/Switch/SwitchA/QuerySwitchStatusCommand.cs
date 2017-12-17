/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLAuto.Device.PLC.Command.Models;
using TLAuto.Device.PLC.Command.Models.Enums;

namespace TLAuto.Device.PLC.Command.Switch.SwitchA
{
    public class QuerySwitchStatusCommand : BoardDeviceCommand<IEnumerable<SwitchItem>>
    {
        public QuerySwitchStatusCommand(int deviceNumber, int itemCount) :
            base(deviceNumber, itemCount)
        {
        }

        protected override bool CheckReveicedDataBody(byte[] bytes)
        {
            if (bytes[1] == 221 && TLAutoBoardDeviceHelper.IsRightCRC(bytes, ResultByteLength))
            {
                return true;
            }
            return false;
        }

        protected override IEnumerable<SwitchItem> GetResult(byte[] bytes)
        {
            var switchItems = new List<SwitchItem>();
            var names = Enum.GetNames(typeof(BoardFlag));
            var list = names.Select(name => (BoardFlag)Enum.Parse(typeof(BoardFlag), name)).ToList();
            var swtichDataIndex = 2;
            var listIndex = 0;
            for (int i = 0; i < ItemCount; i++)
            {
                if (i % 8 == 0)
                {
                    swtichDataIndex += 1;
                }
                var swtichType = list[listIndex];
                switchItems.Add(new SwitchItem(switchItems.Count + 1,
                    ((BoardFlag)bytes[swtichDataIndex] & swtichType) == swtichType ? SwitchStatus.NC : SwitchStatus.NO));
                if (listIndex < list.Count - 1)
                    listIndex++;
                else
                    listIndex = 0;
            }
            return switchItems;
        }

        /// <summary>
        /// 0代表设备号
        /// </summary>
        protected override string SendCommand => "AA BB {0} 0D 02";

        public override bool HasSendData => true;

        public override bool HasReceivedData => true;

        protected override int ResultByteLength => 8;
    }
}
