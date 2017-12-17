// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;

using TLAuto.Device.PLC.Command.Models.Enums;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.Device.PLC.Command.PLC
{
    public class QueryCustom3SwitchStatusCommand : QueryCustom2SwitchStatusCommand
    {
        public QueryCustom3SwitchStatusCommand(int deviceNumber, int itemCount) :
            base(deviceNumber, itemCount) { }

        protected override int ResultByteLength => 6;

        protected override IEnumerable<SwitchItem> GetResult(byte[] bytes)
        {
            var switchItems = new List<SwitchItem>();
            var names = Enum.GetNames(typeof(BoardFlag));
            var list = names.Select(name => (BoardFlag)Enum.Parse(typeof(BoardFlag), name)).ToList();
            var swtichDataIndex = 3;
            var listIndex = 0;
            for (var i = 0; i < ItemCount; i++)
            {
                if (i % 8 == 0)
                {
                    swtichDataIndex += 1;
                }
                var swtichType = list[listIndex];
                switchItems.Add(new SwitchItem(switchItems.Count + 1,
                                               ((BoardFlag)bytes[swtichDataIndex] & swtichType) == swtichType ? SwitchStatus.NC : SwitchStatus.NO));
                if (listIndex < list.Count - 1)
                {
                    listIndex++;
                }
                else
                {
                    listIndex = 0;
                }
            }
            return switchItems;
        }
    }
}