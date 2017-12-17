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
    public class QueryCustom3RelayStatusCommand : QueryCustom2RelayStatusCommand
    {
        public QueryCustom3RelayStatusCommand(int deviceNumber, int itemCount) :
            base(deviceNumber, itemCount) { }

        protected override int ResultByteLength => 6;

        protected override IEnumerable<RelayItem> GetResult(byte[] bytes)
        {
            var relayItems = new List<RelayItem>();
            var names = Enum.GetNames(typeof(BoardFlag));
            var list = names.Select(name => (BoardFlag)Enum.Parse(typeof(BoardFlag), name)).ToList();
            var swtichDataIndex = 2;
            var listIndex = 0;
            for (var i = 0; i < ItemCount; i++)
            {
                if (i % 8 == 0)
                {
                    swtichDataIndex += 1;
                }
                var swtichType = list[listIndex];
                relayItems.Add(new RelayItem(relayItems.Count + 1,
                                             ((BoardFlag)bytes[swtichDataIndex] & swtichType) == swtichType ? RelayStatus.NO : RelayStatus.NC));
                if (listIndex < list.Count - 1)
                {
                    listIndex++;
                }
                else
                {
                    listIndex = 0;
                }
            }
            return relayItems;
        }
    }
}