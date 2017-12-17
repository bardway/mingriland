// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Device.Command
{
    public abstract class DeviceCommand<T>
    {
        /// <summary>
        /// 是否含有发送命令
        /// </summary>
        public abstract bool HasSendData { get; }

        /// <summary>
        /// 是否含有接收命令
        /// </summary>
        public abstract bool HasReceivedData { get; }

        /// <summary>
        /// 发送命令
        /// </summary>
        protected virtual string SendCommand => string.Empty;

        /// <summary>
        /// 获取发送的数据
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetSendData();

        /// <summary>
        /// 验证接收的所有数据
        /// </summary>
        /// <returns></returns>
        public abstract bool ParseReceivedData(byte[] bytes, out T result);

        /// <summary>
        /// 验证接收的所有数据
        /// </summary>
        /// <returns></returns>
        public abstract bool ParseReceivedData(byte[] bytes, out IList<T> results);

        /// <summary>
        /// 获取返回结果
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected virtual T GetResult(byte[] bytes)
        {
            return default(T);
        }
    }
}