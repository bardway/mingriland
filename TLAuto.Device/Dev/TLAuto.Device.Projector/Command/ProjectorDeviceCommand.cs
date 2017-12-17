// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using TLAuto.Device.Command;
#endregion

namespace TLAuto.Device.Projector.Command
{
    public abstract class ProjectorDeviceCommand<T> : DeviceCommand<T>
    {
        private readonly List<byte> _buffer = new List<byte>();

        protected abstract string HeaderCommand { get; }

        public override byte[] GetSendData()
        {
            return TLAutoDeviceHelper.HexStringToByteArray(SendCommand);
        }

        public override bool ParseReceivedData(byte[] bytes, out T result)
        {
            _buffer.AddRange(bytes);
            var checkResult = false;
            result = default(T);
            //头检查
            if (CheckReveicedDataHeader(_buffer.ToArray()))
            {
                var buffers = _buffer.ToArray();
                if (CheckReveicedDataBody(buffers))
                {
                    result = GetResult(buffers);
                    checkResult = true;
                    _buffer.Clear();
                }
            }
            return checkResult;
        }

        public override bool ParseReceivedData(byte[] bytes, out IList<T> results)
        {
            _buffer.AddRange(bytes);
            var checkResult = false;
            results = new List<T>();
            //头检查
            if (CheckReveicedDataHeader(_buffer.ToArray()))
            {
                var buffers = _buffer.ToArray();
                if (CheckReveicedDataBody(buffers))
                {
                    results.Add(GetResult(buffers));
                    checkResult = true;
                    _buffer.Clear();
                }
            }
            return checkResult;
        }

        private bool CheckReveicedDataHeader(byte[] buffer)
        {
            var header = TLAutoDeviceHelper.HexStringToByteArray(HeaderCommand);
            if (buffer.Length < header.Length)
            {
                return false;
            }
            for (var i = 0; i < header.Length; i++)
            {
                if (header[i] != buffer[i])
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual bool CheckReveicedDataBody(byte[] bytes)
        {
            return false;
        }
    }
}