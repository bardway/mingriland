// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;

using TLAuto.Base.Extensions;
using TLAuto.Device.Command;
#endregion

namespace TLAuto.Device.PLC.Command
{
    public abstract class PLCDeviceCommand<T> : DeviceCommand<T>
    {
        private readonly List<byte> _buffer = new List<byte>();

        protected PLCDeviceCommand(int deviceNumber, int itemCount)
        {
            DeviceNumber = deviceNumber;
            ItemCount = itemCount;
        }

        protected virtual int ResultByteLength => 0;

        public int DeviceNumber { get; }

        public int ItemCount { get; }

        protected virtual bool HasSendCRC => true;

        public override byte[] GetSendData()
        {
            var commandParamsList = new List<object> {DeviceNumber.ToHex()};
            var commandParams = GetReplaceCommandParams();
            if (commandParams != null)
            {
                commandParamsList.AddRange(commandParams);
            }
            var commandText = string.Format(SendCommand, commandParamsList.ToArray());
            if (HasSendCRC)
            {
                var crc = TLAutoPLCDeviceHelper.GetCRCFromHexs(commandText);
                commandText = commandText + " " + crc;
            }
            return TLAutoDeviceHelper.HexStringToByteArray(commandText);
        }

        protected virtual IEnumerable<object> GetReplaceCommandParams()
        {
            return null;
        }

        protected virtual bool CheckReveicedDataHeader(byte[] bytes, out int headerLength)
        {
            headerLength = 1;
            return bytes[0] == 204;
        }

        protected virtual bool CheckReveicedDataBody(byte[] bytes)
        {
            return false;
        }

        public override bool ParseReceivedData(byte[] bytes, out T result)
        {
            result = default(T);
            var byteList = bytes.ToList();
            if (_buffer.Count == 0)
            {
                while (byteList.Count != 0)
                {
                    int headerLength;
                    var byteArray = byteList.ToArray();
                    if (CheckReveicedDataHeader(byteArray, out headerLength))
                    {
                        _buffer.AddRange(byteArray);
                        break;
                    }
                    byteList.RemoveRange(0, headerLength);
                }
                if (byteList.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                _buffer.AddRange(bytes);
            }

            var checkResult = false;
            var headerCount = _buffer.Count / ResultByteLength;
            for (var i = 0; i < headerCount; i++)
            {
                var buffer = new List<byte>();
                for (var j = 0; j < ResultByteLength; j++)
                {
                    buffer.Add(_buffer[j]);
                }
                int headerLength;
                //头检查
                if (CheckReveicedDataHeader(buffer.ToArray(), out headerLength))
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
                _buffer.RemoveRange(0, ResultByteLength);
            }
            return checkResult;
        }

        public override bool ParseReceivedData(byte[] bytes, out IList<T> results)
        {
            results = new List<T>();
            var byteList = bytes.ToList();
            if (_buffer.Count == 0)
            {
                while (byteList.Count != 0)
                {
                    int headerLength;
                    var byteArray = byteList.ToArray();
                    if (CheckReveicedDataHeader(byteArray, out headerLength))
                    {
                        _buffer.AddRange(byteArray);
                        break;
                    }
                    byteList.RemoveRange(0, headerLength);
                }
                if (byteList.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                _buffer.AddRange(bytes);
            }

            var checkResult = false;
            var headerCount = _buffer.Count / ResultByteLength;
            for (var i = 0; i < headerCount; i++)
            {
                var buffer = new List<byte>();
                for (var j = 0; j < ResultByteLength; j++)
                {
                    buffer.Add(_buffer[j]);
                }
                int headerLength;
                //头检查
                if (CheckReveicedDataHeader(buffer.ToArray(), out headerLength))
                {
                    var buffers = buffer.ToArray();
                    //主体检查
                    if (CheckReveicedDataBody(buffers))
                    {
                        results.Add(GetResult(buffers));
                        //checkResult = true;
                        //_buffer.Clear();
                        //break;
                    }
                }
                _buffer.RemoveRange(0, ResultByteLength);
            }
            if (results.Count > 0)
            {
                checkResult = true;
            }
            return checkResult;
        }
    }
}