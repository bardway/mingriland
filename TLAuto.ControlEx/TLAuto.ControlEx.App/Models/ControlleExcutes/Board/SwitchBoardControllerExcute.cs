// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
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
    [Description("开关检测执行器")]
    public class SwitchBoardControllerExcute : BoardControllerExcute
    {
        private readonly ConcurrentBag<SendWcfCommand<ITLAutoDevice>> _commands = new ConcurrentBag<SendWcfCommand<ITLAutoDevice>>();

        private volatile bool _isBreak;
        private volatile bool _isStop;

        public SwitchBoardControllerExcute()
        {
            BoardType = BoardType.InputA;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            _isStop = false;
            _isBreak = false;
            var newTasks = new List<Tuple<Task<bool>, BoardItemInfo>>();
            bool[] result;
            do
            {
                newTasks.Clear();
                Parallel.ForEach(BoardItemInfos,
                                 boardItemInfo =>
                                 {
                                     var serviceAddress = ProjectHelper.GetBoardServiceAddress(boardItemInfo.DeviceNumber, BoardType);
                                     var portName = ProjectHelper.GetBoardPortName(boardItemInfo.DeviceNumber, BoardType);
                                     var wcfCommand = new SendWcfCommand<ITLAutoDevice>(serviceAddress, writeLogMsgAction);
                                     _commands.Add(wcfCommand);
                                     var task = wcfCommand.SendAsync(async proxy =>
                                                                     {
                                                                         try
                                                                         {
                                                                             var controlInfo = new ControlInfo {ServiceKey = ConfigHelper.PLCServiceKey};
                                                                             var serviceData = new PLCControlServiceData
                                                                                               {
                                                                                                   ControlPLCType = ControlPLCType.QueryDigitalSwitch,
                                                                                                   DeviceNumber = boardItemInfo.DeviceNumber,
                                                                                                   Number = new[] {boardItemInfo.Number},
                                                                                                   PortSignName = portName
                                                                                               };
                                                                             controlInfo.Data = serviceData.ToBytes();
                                                                             var result1 = await proxy.ControlDevice(controlInfo);
                                                                             if (!result1.IsError && (result1.Data != null))
                                                                             {
                                                                                 var switchItems = result1.Data.ToObject<IEnumerable<SwitchItem>>().ToList();
                                                                                 var switchItem = switchItems.Find(s => (s != null) && (s.SwitchNumber == boardItemInfo.Number) && (s.SwitchStatus.ToInt32().ToBoolean() == !boardItemInfo.IsNo));
                                                                                 if (switchItem != null)
                                                                                 {
                                                                                     return true;
                                                                                 }
                                                                             }
                                                                             serviceData.ControlPLCType = ControlPLCType.QueryDiaitalSwitchWithAutoUpload;
                                                                             serviceData.QueryTimeForAutoUpload = ConfigHelper.QuerySwitchTime;
                                                                             controlInfo.Data = serviceData.ToBytes();
                                                                             var result2 = await proxy.ControlDevice(controlInfo);
                                                                             if (!result2.IsError && (result2.Data != null))
                                                                             {
                                                                                 var switchItems = result2.Data.ToObject<IEnumerable<SwitchItem>>().ToList();
                                                                                 var switchItem = switchItems.Find(s => (s != null) && (s.SwitchNumber == boardItemInfo.Number) && (s.SwitchStatus.ToInt32().ToBoolean() == !boardItemInfo.IsNo));
                                                                                 if (switchItem != null)
                                                                                 {
                                                                                     return true;
                                                                                 }
                                                                             }
                                                                         }
                                                                         catch (Exception ex)
                                                                         {
                                                                             writeLogMsgAction("执行任务时出错，原因为：" + ex.Message);
                                                                         }
                                                                         if (_isBreak)
                                                                         {
                                                                             return true;
                                                                         }
                                                                         return false;
                                                                     });
                                     newTasks.Add(new Tuple<Task<bool>, BoardItemInfo>(task, boardItemInfo));
                                 });
                result = await Task.WhenAll(newTasks.Select(s => s.Item1).ToArray());
                _commands.Clear();
            }
            while (!result.All(s => s) && !_isStop && !_isBreak);
            if (_isStop)
            {
                return false;
            }
            return true;
        }

        private void Close()
        {
            foreach (var command in _commands)
            {
                command.Close();
            }
            _commands.Clear();
        }

        public override void StopExcute(Action<string> writeLogMsgAction)
        {
            try
            {
                _isStop = true;
                Close();
            }
            catch (Exception ex)
            {
                writeLogMsgAction("任务停止出错，原因为：" + ex.Message);
            }
        }

        public override void BreakExcute(Action<string> writeLogMsgAction)
        {
            try
            {
                _isBreak = true;
                Close();
            }
            catch (Exception ex)
            {
                writeLogMsgAction("任务跳过过程中对任务进行关闭出错，原因为：" + ex.Message);
            }
        }
    }
}