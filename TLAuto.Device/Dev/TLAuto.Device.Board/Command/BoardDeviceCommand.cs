/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLAuto.Base.Extensions;
using TLAuto.Device.Command;

namespace TLAuto.Device.PLC.Command
{
    public abstract class BoardDeviceCommand<T> : DeviceCommand<T>
    {
        private readonly List<byte> _buffer = new List<byte>();

        protected BoardDeviceCommand(int deviceNumber, int itemCount)
        {
            DeviceNumber = deviceNumber;
            ItemCount = itemCount;
        }

        public override byte[] GetSendData()
        {
            var deviceNumberHex = DeviceNumber.ToHex();
            var commandParamsList = new List<object> { deviceNumberHex };
            var commandParams = GetReplaceCommandParams();
            if (commandParams != null)
            {
                commandParamsList.AddRange(commandParams);
            }
            var commandText = string.Format(SendCommand, commandParamsList.ToArray());
            var crc = TLAutoBoardDeviceHelper.GetCRCFromHexs(commandText);
            var sendText = commandText + " " + crc;
            return TLAutoDeviceHelper.HexStringToByteArray(sendText);
        }

        protected virtual IEnumerable<object> GetReplaceCommandParams()
        {
            return null;
        }

        protected virtual bool CheckReveicedDataHeader(byte[] bytes)
        {
            return bytes[0] == 204;
        }

        protected virtual bool CheckReveicedDataBody(byte[] bytes)
        {
            return false;
        }

        public override bool ParseReceivedData(byte[] bytes, out T result)
        {
            _buffer.AddRange(bytes);
            var checkResult = false;
            result = default(T);
            var headerCount = _buffer.Count / ResultByteLength;
            for (int i = 0; i < headerCount; i++)
            {
                var headerStartLength = ResultByteLength * i;
                var buffer = new List<byte>();
                for (int j = headerStartLength; j < headerStartLength + ResultByteLength; j++)
                {
                    buffer.Add(_buffer[j]);
                }
                //头检查
                if (CheckReveicedDataHeader(buffer.ToArray()))
                {
                    var buffers = buffer.ToArray();
                    //主体检查
                    if (CheckReveicedDataBody(buffers))
                    {
                        result = GetResult(buffers);
                        checkResult = true;
                        _buffer.Clear();
                        break;
                    }
                }
            }
            return checkResult;
        }

        protected virtual int ResultByteLength => 0;

        public int DeviceNumber { get; }

        public int ItemCount { private set; get; }
    }
}
