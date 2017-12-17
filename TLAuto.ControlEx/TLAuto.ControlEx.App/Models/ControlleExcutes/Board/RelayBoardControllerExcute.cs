// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.Device.Contracts;
using TLAuto.Device.PLC.ServiceData;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Board
{
    [Description("继电器控制执行器")]
    public class RelayBoardControllerExcute : BoardControllerExcute
    {
        public RelayBoardControllerExcute()
        {
            BoardType = BoardType.OutputA;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var tasks = new List<bool>();
            foreach (var boardItemInfo in BoardItemInfos)
            {
                var serviceAddress = ProjectHelper.GetBoardServiceAddress(boardItemInfo.DeviceNumber, BoardType);
                var portName = ProjectHelper.GetBoardPortName(boardItemInfo.DeviceNumber, BoardType);
                var wcfCommand = new SendWcfCommand<ITLAutoDevice>(serviceAddress, writeLogMsgAction);
                var task = await wcfCommand.SendAsync(async proxy =>
                                                      {
                                                          try
                                                          {
                                                              var controlInfo = new ControlInfo {ServiceKey = ConfigHelper.PLCServiceKey};
                                                              var serviceData = new PLCControlServiceData
                                                                                {
                                                                                    ControlPLCType = ControlPLCType.ControlRelay,
                                                                                    DeviceNumber = boardItemInfo.DeviceNumber,
                                                                                    Number = new[] {boardItemInfo.Number},
                                                                                    PortSignName = portName,
                                                                                    RelayStatus = boardItemInfo.IsNo
                                                                                };
                                                              controlInfo.Data = serviceData.ToBytes();
                                                              var result = await proxy.ControlDevice(controlInfo);
                                                              if (!result.IsError && (result.Data != null))
                                                              {
                                                                  return true;
                                                              }
                                                          }
                                                          catch (Exception ex)
                                                          {
                                                              writeLogMsgAction("执行任务时出错，原因为：" + ex.Message);
                                                          }
                                                          return false;
                                                      });
                tasks.Add(task);
            }
            return tasks.All(s => s);
        }
    }
}